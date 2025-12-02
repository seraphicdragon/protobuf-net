
using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace ProtoBuf.Internal.Serializers
{
    public class FieldDecorator : ProtoDecoratorBase
    {
        protected static readonly Dictionary<Type, ConstructorInfo> fieldDecoratorConstructors = new Dictionary<Type, ConstructorInfo>();
        public override Type ExpectedType { get; }

        protected readonly FieldInfo field;

        public override bool RequiresOldValue => true;
        public override bool ReturnsValue => false;

        public FieldDecorator(ValueMember valueMember, Type forType, FieldInfo field, IRuntimeProtoSerializerNode tail) : base(valueMember, tail)
        {
            if (tail is null) ThrowHelper.ThrowArgumentNullException(nameof(tail));
            if (field is null) ThrowHelper.ThrowArgumentNullException(nameof(field));
            if (forType is null) ThrowHelper.ThrowArgumentNullException(nameof(forType));
            ExpectedType = forType;
            this.field = field;
        }

        public override void Write(ref ProtoWriter.State state, object value)
        {
            Debug.Assert(value is not null);
            ICustomDecoratorSerializable serializable = value as ICustomDecoratorSerializable;
            ProtobufProfiler.BeginProfile(value.GetType(), ProfilerType.Write);
            try
            {
                if (serializable != null && serializable.TrySerializeMember(ValueMember))
                {
                    TagDecorator tagDecorator = Tail as TagDecorator;


                    if (tagDecorator != null)
                    {
                        tagDecorator.WriteFieldHeader(ref state);

                        if(ProtobufProfiler.ProfileFieldLevel)
                            ProtobufProfiler.BeginProfile(field);
                        try
                        {
                            if (!serializable.TryWrite(ref state, ValueMember, EndTail))
                            {
                                throw new InvalidOperationException("FieldDecorator.Write = Failed to write this Field ID: " + ValueMember.FieldNumber + " for this type: " + serializable.GetType());
                            }
                        }
                        finally
                        {
                            if (ProtobufProfiler.ProfileFieldLevel)
                                ProtobufProfiler.EndProfiler();
                        }
                    }
                    else
                    {
                        DefaultValueDecorator defaultValueDecorator = Tail as DefaultValueDecorator;
                        if (defaultValueDecorator != null)
                        {
                            if (!serializable.IsDefault(ValueMember))
                            {
                                TagDecorator defaultTagDecorator = (TagDecorator)defaultValueDecorator.Tail;
                                defaultTagDecorator.WriteFieldHeader(ref state);
                                if (ProtobufProfiler.ProfileFieldLevel)
                                    ProtobufProfiler.BeginProfile(field);
                                try
                                {
                                    if (!serializable.TryWrite(ref state, ValueMember, EndTail))
                                    throw new InvalidOperationException("FieldDecorator.Write = Failed to write this Field ID: " + ValueMember.FieldNumber + " for this type: " + serializable.GetType());
                                }
                                finally
                                {
                                    if (ProtobufProfiler.ProfileFieldLevel)
                                        ProtobufProfiler.EndProfiler();
                                }
                            }
                            // Tail.Write(ref state, value);
                        }
                        else
                        {
                            //throw new InvalidOperationException("Failed to parse thie decorator's type: " + Tail.GetType());
                            bool successfulWrite = false;
                            ProtobufProfiler.BeginProfile(value.GetType(), ProfilerType.GetField);
                            try
                            {
                                if (ProtobufProfiler.ProfileFieldLevel)
                                    ProtobufProfiler.BeginProfile(field);
                                try
                                {
                                    if (!(successfulWrite = serializable.TryWrite(ref state, ValueMember, Tail)))
                                        value = field.GetValue(value);
                                }
                                finally
                                {
                                    if (ProtobufProfiler.ProfileFieldLevel)
                                        ProtobufProfiler.EndProfiler();
                                }
                            }
                            finally
                            {
                                ProtobufProfiler.EndProfiler();
                            }

                            if (!successfulWrite)
                            {
                                ProtobufProfiler.BeginProfile(value.GetType(), ProfilerType.TailWrite);
                                try
                                {
                                    if (value is not null)
                                        Tail.Write(ref state, value);
                                }
                                finally
                                {
                                    ProtobufProfiler.EndProfiler();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ProtobufProfiler.BeginProfile(value.GetType(), ProfilerType.GetField);
                    try
                    {
                        if (ProtobufProfiler.ProfileFieldLevel)
                            ProtobufProfiler.BeginProfile(field);
                        try
                        {
                            value = field.GetValue(value);
                        }
                        finally
                        {
                            if (ProtobufProfiler.ProfileFieldLevel)
                                ProtobufProfiler.EndProfiler();
                        }
                    }
                    finally
                    {
                        ProtobufProfiler.EndProfiler();
                    }
                    if (value is not null)
                    {
                        if (ProtobufProfiler.IsProfiling && value.GetType().IsValueType)
                            ProtobufProfiler.BeginProfile(value.GetType(), ProfilerType.ValueTypeBoxing);
                        try
                        {
                            Tail.Write(ref state, value);
                        }
                        finally
                        {
                            if (ProtobufProfiler.IsProfiling && value.GetType().IsValueType)
                                ProtobufProfiler.EndProfiler();
                        }
                    }
                }
            }
            finally
            {
                ProtobufProfiler.EndProfiler();
            }
        }

        public override object Read(ref ProtoReader.State state, object value)
        {
            ICustomDecoratorSerializable serializable = value as ICustomDecoratorSerializable;
            if (serializable != null && serializable.TrySerializeMember(ValueMember))
            {
                //  Debug.Assert(fieldNumber == state.FieldNumber);
                TagDecorator tagDecorator = Tail as TagDecorator;
                if (tagDecorator != null)
                {
                    if (tagDecorator.IsStrict) { state.Assert(tagDecorator.WireType); }
                    else if (tagDecorator.NeedsHint) { state.Hint(tagDecorator.WireType); }


                    if (!serializable.TryRead(ref state, ValueMember, EndTail))
                    {
                        throw new InvalidOperationException("FieldDecorator.Write = Failed to read this Field ID: " + ValueMember.FieldNumber + " for this type: " + serializable.GetType());
                    }
                }
                else
                {
                    DefaultValueDecorator defaultValueDecorator = Tail as DefaultValueDecorator;
                    if (defaultValueDecorator != null)
                    {
                        if (!serializable.TryRead(ref state, ValueMember, EndTail))
                            throw new InvalidOperationException("FieldDecorator.Read = Failed to read this Field ID: " + ValueMember.FieldNumber + " for this type: " + serializable.GetType());
                        // Tail.Write(ref state, value);
                    }
                    else
                    {
                        /* throw new InvalidOperationException("Failed to parse thie decorator's type: " + Tail.GetType());*/
                        Debug.Assert(value is not null);
                        object newValue = Tail.Read(ref state, Tail.RequiresOldValue ? field.GetValue(value) : null);
                        if (newValue is not null) field.SetValue(value, newValue);
                        return null;
                    }
                }
                return serializable;
            }
            else
            {
                Debug.Assert(value is not null);
                object newValue = Tail.Read(ref state, Tail.RequiresOldValue ? field.GetValue(value) : null);
                if (newValue is not null) field.SetValue(value, newValue);
                return null;
            }
        }

        protected override void EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.LoadAddress(valueFrom, ExpectedType);
            ctx.LoadValue(field);
            ctx.WriteNullCheckedTail(field.FieldType, Tail, null);
        }
        protected override void EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            using Compiler.Local loc = ctx.GetLocalWithValue(ExpectedType, valueFrom);
            if (Tail.RequiresOldValue)
            {
                ctx.LoadAddress(loc, ExpectedType);
                ctx.LoadValue(field);
            }
            // value is either now on the stack or not needed
            ctx.ReadNullCheckedTail(field.FieldType, Tail, null);

            // the field could be a backing field that needs to be raised back to
            // the property if we're doing a full compile
            MemberInfo member = field;
            ctx.CheckAccessibility(ref member);
            bool writeValue = member is FieldInfo;

            if (writeValue)
            {
                if (Tail.ReturnsValue)
                {
                    var localType = PropertyDecorator.ChooseReadLocalType(field.FieldType, Tail.ExpectedType);
                    using Compiler.Local newVal = new Compiler.Local(ctx, localType);
                    ctx.StoreValue(newVal);
                    if (field.FieldType.IsValueType)
                    {
                        ctx.LoadAddress(loc, ExpectedType);
                        ctx.LoadValue(newVal);
                        ctx.StoreValue(field);
                    }
                    else
                    {
                        Compiler.CodeLabel allDone = ctx.DefineLabel();
                        ctx.LoadValue(newVal);
                        ctx.BranchIfFalse(allDone, true); // interpret null as "don't assign"

                        ctx.LoadAddress(loc, ExpectedType);
                        ctx.LoadValue(newVal);

                        // cast if needed (this is mostly for ReadMap/ReadRepeated)
                        if (!field.FieldType.IsValueType && !localType.IsValueType
                            && !field.FieldType.IsAssignableFrom(localType))
                        {
                            ctx.Cast(field.FieldType);
                        }

                        ctx.StoreValue(field);
                        ctx.MarkLabel(allDone);
                    }
                }
            }
            else
            {
                // can't use result
                if (Tail.ReturnsValue)
                {
                    ctx.DiscardValue();
                }
            }
        }



        internal static FieldDecorator CreateInstance(Type memberType, ValueMember valueMember, Type forType, FieldInfo field, IRuntimeProtoSerializerNode tail)
        {
            if (fieldDecoratorConstructors.TryGetValue(memberType, out ConstructorInfo constructor))
            {

                // Example of invoking the constructor
                object[] constructorArgs = new object[] { valueMember, forType, field, tail };
                object instance = constructor.Invoke(constructorArgs);

                return instance as FieldDecorator;
            }
            throw new InvalidOperationException("Failed to find FieldDecorator constructor for this type: " + memberType);
        }
    }

    public sealed class FieldDecorator<T> : FieldDecorator, IRuntimeProtoSerializerNode<T> where T : ICustomDecoratorSerializable
    {
        private static readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        public FieldDecorator(ValueMember valueMember, Type forType, FieldInfo field, IRuntimeProtoSerializerNode tail) : base(valueMember, forType, field, tail)
        {
        }

        public void Write(ref ProtoWriter.State state, T value)
        {
            if (value.TrySerializeMember(ValueMember))
            {
                TagDecorator tagDecorator = Tail as TagDecorator;


                if (tagDecorator != null)
                {
                    tagDecorator.WriteFieldHeader(ref state);


                    if (!value.TryWrite(ref state, ValueMember, EndTail))
                    {
                        throw new InvalidOperationException("FieldDecorator.Write = Failed to write this Field ID: " + ValueMember.FieldNumber + " for this type: " + value.GetType());
                    }
                }
                else
                {
                    DefaultValueDecorator defaultValueDecorator = Tail as DefaultValueDecorator;
                    if (defaultValueDecorator != null)
                    {
                        if (!value.IsDefault(ValueMember))
                        {
                            TagDecorator defaultTagDecorator = (TagDecorator)defaultValueDecorator.Tail;
                            defaultTagDecorator.WriteFieldHeader(ref state);
                            if (!value.TryWrite(ref state, ValueMember, EndTail))
                                throw new InvalidOperationException("FieldDecorator.Write = Failed to write this Field ID: " + ValueMember.FieldNumber + " for this type: " + value.GetType());
                        }
                        // Tail.Write(ref state, value);
                    }
                    else
                    {
                        /*throw new InvalidOperationException("Failed to parse thie decorator's type: " + Tail.GetType());*/
                        //value = (T)field.GetValue(value);
                        //Tail.Write(ref state, value);
                        if (!value.TryWrite(ref state, ValueMember, Tail))
                            throw new InvalidOperationException("FieldDecorator.Write = Failed to write this Field ID: " + ValueMember.FieldNumber + " for this type: " + value.GetType());
                    }
                }
            }
            else
            {
                value = (T)field.GetValue(value);
                Tail.Write(ref state, value);
            }
        }

        public T Read(ref ProtoReader.State state, T serializable)
        {
            if (serializable.TrySerializeMember(ValueMember))
            {
                //  Debug.Assert(fieldNumber == state.FieldNumber);
                TagDecorator tagDecorator = Tail as TagDecorator;
                if (tagDecorator != null)
                {
                    if (tagDecorator.IsStrict) { state.Assert(tagDecorator.WireType); }
                    else if (tagDecorator.NeedsHint) { state.Hint(tagDecorator.WireType); }


                    if (!serializable.TryRead(ref state, ValueMember, EndTail))
                    {
                        throw new InvalidOperationException("FieldDecorator.Write = Failed to read this Field ID: " + ValueMember.FieldNumber + " for this type: " + serializable.GetType());
                    }
                }
                else
                {
                    DefaultValueDecorator defaultValueDecorator = Tail as DefaultValueDecorator;
                    if (defaultValueDecorator != null)
                    {
                        if (!serializable.TryRead(ref state, ValueMember, EndTail))
                            throw new InvalidOperationException("Error occurred!");
                        // Tail.Write(ref state, value);
                    }
                    else
                    {
                        //throw new InvalidOperationException("Failed to parse thie decorator's type: " + Tail.GetType());
                        //  object newValue = Tail.Read(ref state, Tail.RequiresOldValue ? field.GetValue(serializable) : null);
                        //  if (newValue is not null) field.SetValue(serializable, newValue);
                        //  return (T)newValue;
                        serializable.TryRead(ref state, ValueMember, Tail);
                    }
                }
                return serializable;
            }
            else
            {

                object newValue = Tail.Read(ref state, Tail.RequiresOldValue ? field.GetValue(serializable) : null);
                if (newValue is not null) field.SetValue(serializable, newValue);
                return (T)newValue;
            }
        }

        public static void CreateType()
        { // Get the Type object for the constructed generic type (e.g., GenericClass<int>)
            if (fieldDecoratorConstructors.ContainsKey(typeof(T)))
                return;
            Type constructedGenericType = typeof(FieldDecorator<T>);

            // Define the parameter types for the desired constructor
            Type[] constructorParameterTypes = new Type[] { typeof(ValueMember), typeof(Type), typeof(FieldInfo), typeof(IRuntimeProtoSerializerNode) };

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
                throw new InvalidOperationException("Failed to find a constructor for this type: " +  typeof(FieldDecorator<T>));
            }
            fieldDecoratorConstructors.Add(typeof(T), constructor);
        }
    }

}