using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ProtoBuf.Internal.Serializers
{
    internal class TagDecorator : ProtoDecoratorBase, IProtoTypeSerializer
    {
        protected static readonly Dictionary<Type, ConstructorInfo> defaultValueDecoratorConstructors = new Dictionary<Type, ConstructorInfo>();
        SerializerFeatures IProtoTypeSerializer.Features => wireType.AsFeatures();
        bool IProtoTypeSerializer.IsSubType => Tail is IProtoTypeSerializer pts && pts.IsSubType;
        public bool HasCallbacks(TypeModel.CallbackType callbackType) => Tail is IProtoTypeSerializer pts && pts.HasCallbacks(callbackType);

        public bool CanCreateInstance() => Tail is IProtoTypeSerializer pts && pts.CanCreateInstance();

        public object CreateInstance(ISerializationContext source) => ((IProtoTypeSerializer)Tail).CreateInstance(source);

        public void Callback(object value, TypeModel.CallbackType callbackType, ISerializationContext context)
            => (Tail as IProtoTypeSerializer)?.Callback(value, callbackType, context);
            
        public void EmitCallback(Compiler.CompilerContext ctx, Compiler.Local valueFrom, TypeModel.CallbackType callbackType)
        {
            // we only expect this to be invoked if HasCallbacks returned true, so implicitly Tail
            // **must** be of the correct type
            ((IProtoTypeSerializer)Tail).EmitCallback(ctx, valueFrom, callbackType);
        }

        public void EmitCreateInstance(Compiler.CompilerContext ctx, bool callNoteObject)
        {
            ((IProtoTypeSerializer)Tail).EmitCreateInstance(ctx, callNoteObject);
        }

        bool IProtoTypeSerializer.ShouldEmitCreateInstance => Tail is IProtoTypeSerializer pts && pts.ShouldEmitCreateInstance;

        public override Type ExpectedType => Tail.ExpectedType;
        Type IProtoTypeSerializer.BaseType => ExpectedType;

        public TagDecorator(ValueMember valueMember, int fieldNumber, WireType wireType, bool strict, IRuntimeProtoSerializerNode tail)
            : base(valueMember, tail)
        {
            this.fieldNumber = fieldNumber;
            this.wireType = wireType;
            this.strict = strict;
        }

        public override bool RequiresOldValue => Tail.RequiresOldValue;

        public override bool ReturnsValue => Tail.ReturnsValue;

        protected readonly bool strict;
        protected readonly int fieldNumber;
        protected readonly WireType wireType;

        public bool NeedsHint => ((int)wireType & ~7) != 0;

        public override object Read(ref ProtoReader.State state, object value)
        {
            Debug.Assert(fieldNumber == state.FieldNumber);
            if (strict) { state.Assert(wireType); }
            else if (NeedsHint) { state.Hint(wireType); }
            return Tail.Read(ref state, value);
        }

        public override void Write(ref ProtoWriter.State state, object value)
        {
            if (Tail is IDirectRuntimeWriteNode dw && dw.CanDirectWrite(wireType))
            {
                dw.DirectWrite(fieldNumber, wireType, ref state, value);
            }
            else
            {
                state.WriteFieldHeader(fieldNumber, wireType);
                Tail.Write(ref state, value);
            }
        }

        public void WriteFieldHeader(ref ProtoWriter.State state)
        {
            state.WriteFieldHeader(fieldNumber, wireType);
        }

        bool IProtoTypeSerializer.HasInheritance => false;

        void IProtoTypeSerializer.EmitReadRoot(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
            => EmitRead(ctx, valueFrom);

        void IProtoTypeSerializer.EmitWriteRoot(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
            => EmitWrite(ctx, valueFrom);

        protected override void EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            if (Tail is IDirectWriteNode dw && dw.CanEmitDirectWrite(wireType))
            {
                dw.EmitDirectWrite(fieldNumber, wireType, ctx, valueFrom);
            }
            else
            {
                ctx.LoadState();
                ctx.LoadValue((int)fieldNumber);
                ctx.LoadValue((int)wireType);
                ctx.EmitCall(typeof(ProtoWriter.State).GetMethod(nameof(ProtoWriter.State.WriteFieldHeader)));
                Tail.EmitWrite(ctx, valueFrom);
            }
        }

        public bool CanEmitDirectWrite()
            => Tail is IDirectWriteNode dw && dw.CanEmitDirectWrite(wireType);

        public void EmitDirectWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
            => ((IDirectWriteNode)Tail).EmitDirectWrite(fieldNumber, wireType, ctx, valueFrom);

        protected override void EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            if (strict || NeedsHint)
            {
                ctx.LoadState();
                ctx.LoadValue((int)wireType);
                string name = strict ? nameof(ProtoReader.State.Assert) : nameof(ProtoReader.State.Hint);
                ctx.EmitCall(typeof(ProtoReader.State).GetMethod(name, new[] { typeof(WireType) }));
            }
            Tail.EmitRead(ctx, valueFrom);
        }
        internal static DefaultValueDecorator CreateInstance(Type memberType, ValueMember valueMember, int fieldNumber, WireType wireType, bool strict, IRuntimeProtoSerializerNode tail)
        {
            //ValueMember valueMember, IRuntimeProtoSerializerNode tail
            if (defaultValueDecoratorConstructors.TryGetValue(memberType, out ConstructorInfo constructor))
            {

                // Example of invoking the constructor
                object[] constructorArgs = new object[] { valueMember, fieldNumber, wireType, strict, tail };
                object instance = constructor.Invoke(constructorArgs);

                return instance as DefaultValueDecorator;
            }
            throw new InvalidOperationException("Failed to find FieldDecorator constructor for this type: " + memberType);
        }

        public bool IsStrict => strict;

        public WireType WireType => wireType;
    }

    internal sealed class TagDecorator<T> : TagDecorator, IRuntimeProtoSerializerNode<T>
    {
        private readonly IRuntimeProtoSerializerNode<T> castedTail = null;
        public TagDecorator(ValueMember valueMember, int fieldNumber, WireType wireType, bool strict, IRuntimeProtoSerializerNode tail) : base(valueMember, fieldNumber, wireType, strict, tail)
        {
            castedTail = tail as IRuntimeProtoSerializerNode<T>;
        }

        public T Read(ref ProtoReader.State state, T value)
        {
            Debug.Assert(fieldNumber == state.FieldNumber);
            if (strict) { state.Assert(wireType); }
            else if (NeedsHint) { state.Hint(wireType); }

            if(castedTail != null)
                return castedTail.Read(ref state, value);
            else
                return (T)Tail.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, T value)
        {
            if (Tail is IDirectRuntimeWriteNode dw && dw.CanDirectWrite(wireType))
            {
                dw.DirectWrite(fieldNumber, wireType, ref state, value);
            }
            else
            {
                state.WriteFieldHeader(fieldNumber, wireType);

                if (castedTail != null)
                    castedTail.Write(ref state, value);
                else
                    Tail.Write(ref state, value);
            }
        }

        /*internal static void CreateType()
        { // Get the Type object for the constructed generic type (e.g., GenericClass<int>)
            Type constructedGenericType = typeof(TagDecorator<T>);

            // Define the parameter types for the desired constructor
            //TagDecorator(ValueMember valueMember, int fieldNumber, WireType wireType, bool strict, IRuntimeProtoSerializerNode tail) 
            Type[] constructorParameterTypes = new Type[] { typeof(ValueMember), typeof(int), typeof(WireType), typeof(bool), typeof(IRuntimeProtoSerializerNode) };

            // Get the public constructor with the specified parameter types
            ConstructorInfo constructor = constructedGenericType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null, // Binder (optional, use null for default)
                constructorParameterTypes,
                null // Parameter modifiers (optional, use null)
            );

            if (constructor != null)
            {

            }
            else
            {
                throw new InvalidOperationException("Failed to find a constructor for this type: " + typeof(DefaultValueDecorator<T>));
            }
            defaultValueDecoratorConstructors.Add(typeof(T), constructor);
        }*/
    }
}