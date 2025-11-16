using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf.Internal.Serializers
{
    internal class EnumMemberSerializer : IRuntimeProtoSerializerNode, IDirectWriteNode,
        IRuntimeProtoSerializerNode<sbyte>,
        IRuntimeProtoSerializerNode<short>,
        IRuntimeProtoSerializerNode<int>,
        IRuntimeProtoSerializerNode<long>,
        IRuntimeProtoSerializerNode<byte>,
        IRuntimeProtoSerializerNode<ushort>,
        IRuntimeProtoSerializerNode<uint>,
        IRuntimeProtoSerializerNode<ulong>
    {

        protected static readonly Dictionary<Type, ConstructorInfo> enumDecoratorConstructors = new Dictionary<Type, ConstructorInfo>();

        bool IRuntimeProtoSerializerNode.IsScalar => true;

        private readonly IRuntimeProtoSerializerNode _tail;
        public EnumMemberSerializer(Type enumType)
        {
            ExpectedType = enumType ?? throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) ThrowHelper.ThrowInvalidOperationException("Expected an enum type; got " + enumType.NormalizeName());
            
            _tail = Type.GetTypeCode(enumType) switch
            {
                TypeCode.SByte => SByteSerializer.Instance,
                TypeCode.Int16 => Int16Serializer.Instance,
                TypeCode.Int32 => Int32Serializer.Instance,
                TypeCode.Int64 => Int64Serializer.Instance,
                TypeCode.Byte => ByteSerializer.Instance,
                TypeCode.UInt16 => UInt16Serializer.Instance,
                TypeCode.UInt32 => UInt32Serializer.Instance,
                TypeCode.UInt64 => UInt64Serializer.Instance,
                _ => default,
            };
            if (_tail is null) ThrowHelper.ThrowInvalidOperationException("Unable to resolve underlying enum type for " + enumType.NormalizeName());

        }

        public Type ExpectedType { get; }

        bool IRuntimeProtoSerializerNode.RequiresOldValue => false;

        bool IRuntimeProtoSerializerNode.ReturnsValue => true;

        internal static object EnumToWire(object value, Type type)
        {
            unchecked
            {
                return Type.GetTypeCode(type) switch
                { // unbox as the intended type
                    TypeCode.Byte => (byte)value,
                    TypeCode.SByte => (sbyte)value,
                    TypeCode.Int16 => (short)value,
                    TypeCode.Int32 => (int)value,
                    TypeCode.Int64 => (long)value,
                    TypeCode.UInt16 => (ushort)(ushort)value,
                    TypeCode.UInt32 => (uint)(uint)value,
                    TypeCode.UInt64 => (ulong)(ulong)value,
                    _ => throw new InvalidOperationException(),
                };
            }
        }
        private object EnumToWire(object value) => EnumToWire(value, ExpectedType);

        public object Read(ref ProtoReader.State state, object value)
            => Enum.ToObject(ExpectedType, _tail.Read(ref state, value));

        public void Write(ref ProtoWriter.State state, object value)
            => _tail.Write(ref state, EnumToWire(value));

        void IRuntimeProtoSerializerNode.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
            => _tail.EmitWrite(ctx, valueFrom);

        void IRuntimeProtoSerializerNode.EmitRead(Compiler.CompilerContext ctx, Compiler.Local entity)
            => _tail.EmitRead(ctx, entity);

        bool IDirectWriteNode.CanEmitDirectWrite(WireType wireType) => _tail is IDirectWriteNode dw && dw.CanEmitDirectWrite(wireType);

        void IDirectWriteNode.EmitDirectWrite(int fieldNumber, WireType wireType, CompilerContext ctx, Local valueFrom)
            => ((IDirectWriteNode)_tail).EmitDirectWrite(fieldNumber, wireType, ctx, valueFrom);

        public void Write(ref ProtoWriter.State state, in sbyte value)
        {
            SByteSerializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref sbyte value)
        {
            SByteSerializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in short value)
        {
            Int16Serializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref short value)
        {
            Int16Serializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in int value)
        {
            Int32Serializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref int value)
        {
            Int32Serializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in long value)
        {
            Int64Serializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref long value)
        {
            Int64Serializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in byte value)
        {
            ByteSerializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state,  ref byte value)
        {
            ByteSerializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, in ushort value)
        {
            UInt16Serializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref ushort value)
        {
            UInt16Serializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in uint value)
        {
            UInt32Serializer.Instance.Write(ref state, value);
        }

        public void Read(ref ProtoReader.State state, ref uint value)
        {
            UInt32Serializer.Instance.Read(ref state, ref value);
        }

        public void Write(ref ProtoWriter.State state, in ulong value)
        {
            UInt64Serializer.Instance.Write(ref state, in value);
        }

        public void Read(ref ProtoReader.State state, ref ulong value)
        {
            UInt64Serializer.Instance.Read(ref state, value);
        }

        internal static bool TryCreateInstance(Type enumType, out EnumMemberSerializer value)
        {
            if (enumDecoratorConstructors.TryGetValue(enumType, out ConstructorInfo constructor))
            {

                // Example of invoking the constructor
                object[] constructorArgs = new object[] { enumType };
                value = (EnumMemberSerializer)constructor.Invoke(constructorArgs);

                return value != null;
            }
            value = null;
            return false;
            //throw new InvalidOperationException("Failed to find FieldDecorator constructor for this type: " + enumType);
        }
    }

    internal class EnumMemberSerializer<T> : EnumMemberSerializer, IRuntimeProtoSerializerNode<T> where T : Enum
    {
        private static readonly T[] enumValues = (T[])Enum.GetValues(typeof(T));
        private static Dictionary<T, object> enumToStoredObjects = new Dictionary<T, object>(enumValues.Length);
        private static Dictionary<object, T> objectToEnum = new Dictionary<object, T>(enumValues.Length);
        private static readonly IDictionary primitivesToType;
        private static readonly IDictionary typeToPrimitives;

        static EnumMemberSerializer()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.SByte:
                    primitivesToType = new Dictionary<sbyte, T>();
                    typeToPrimitives = new Dictionary<T, sbyte>();
                    break;
                case TypeCode.Int16:
                    primitivesToType = new Dictionary<short, T>();
                    typeToPrimitives = new Dictionary<T, short>();
                    break;
                case TypeCode.Int32:
                    primitivesToType = new Dictionary<int, T>();
                    typeToPrimitives = new Dictionary<T, int>();
                    break;
                case TypeCode.Int64:
                    primitivesToType = new Dictionary<long, T>();
                    typeToPrimitives = new Dictionary<T, long>();
                    break;
                case TypeCode.Byte:
                    primitivesToType = new Dictionary<byte, T>();
                    typeToPrimitives = new Dictionary<T, byte>();
                    break;
                case TypeCode.UInt16:
                    primitivesToType = new Dictionary<ushort, T>();
                    typeToPrimitives = new Dictionary<T, ushort>();
                    break;
                case TypeCode.UInt32:
                    primitivesToType = new Dictionary<uint, T>();
                    typeToPrimitives = new Dictionary<T, uint>();
                    break;
                case TypeCode.UInt64:
                    primitivesToType = new Dictionary<ulong, T>();
                    typeToPrimitives = new Dictionary<T, ulong>();
                    break;
                default:
                throw new InvalidOperationException("Failed to parse this type: " + Type.GetTypeCode(typeof(T)));
            }
            for (int i = 0; i < enumValues.Length; i++)
            {
                T value = enumValues[i];
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.SByte:
                        enumToStoredObjects.Add(value, Convert<sbyte>(value));
                        objectToEnum.Add(Convert<sbyte>(value), value);
                        break;
                    case TypeCode.Int16:
                        enumToStoredObjects.Add(value, Convert<short>(value));
                        objectToEnum.Add(Convert<short>(value), value);
                        break;
                    case TypeCode.Int32:
                        enumToStoredObjects.Add(value, Convert<int>(value));
                        objectToEnum.Add(Convert<int>(value), value);
                        break;
                    case TypeCode.Int64:
                        enumToStoredObjects.Add(value, Convert<long>(value));
                        objectToEnum.Add(Convert<long>(value), value);
                        break;
                    case TypeCode.Byte:
                        enumToStoredObjects.Add(value, Convert<byte>(value));
                        objectToEnum.Add(Convert<byte>(value), value);
                        break;
                    case TypeCode.UInt16:
                        enumToStoredObjects.Add(value, Convert<ushort>(value));
                        objectToEnum.Add(Convert<ushort>(value), value);
                        break;
                    case TypeCode.UInt32:
                        enumToStoredObjects.Add(value, Convert<uint>(value));
                        objectToEnum.Add(Convert<uint>(value), value);
                        break;
                    case TypeCode.UInt64:
                        enumToStoredObjects.Add(value, Convert<ulong>(value));
                        objectToEnum.Add(Convert<ulong>(value), value);
                        break;
                    default:
                        throw new InvalidOperationException("Failed to parse this type: " + Type.GetTypeCode(typeof(T)));
                }
            }
        }

        public EnumMemberSerializer(Type enumType) : base(enumType)
        {
        }

        public void Read(ref ProtoReader.State state, ref T value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.SByte:
                    value = Convert(SByteSerializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.Int16:
                    value = Convert(Int16Serializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.Int32:
                    value = Convert(Int32Serializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.Int64:
                    value = Convert(Int64Serializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.Byte:
                    value = Convert(ByteSerializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.UInt16:
                    value = Convert(UInt16Serializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.UInt32:
                    value = Convert(UInt32Serializer.Instance.Read(ref state, default));
                    break;
                case TypeCode.UInt64:
                    value = Convert(UInt64Serializer.Instance.Read(ref state, default));
                    break;
                default:
                    throw new InvalidOperationException("Failed to parse this type: " + Type.GetTypeCode(typeof(T)));
            }
        }

        public void Write(ref ProtoWriter.State state, in T value)
        {
            switch( Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.SByte:
                    SByteSerializer.Instance.Write(ref state, Convert<sbyte>(in value));
                    break;
                case TypeCode.Int16:
                    Int16Serializer.Instance.Write(ref state, Convert<short>(in value));
                    break;
                case TypeCode.Int32:
                    Int32Serializer.Instance.Write(ref state, Convert<int>(in value));
                    break;
                case TypeCode.Int64:
                    Int64Serializer.Instance.Write(ref state, Convert<long>(in value));
                    break;
                case TypeCode.Byte:
                    ByteSerializer.Instance.Write(ref state, Convert<byte>(in value));
                    break;
                case TypeCode.UInt16:
                    UInt16Serializer.Instance.Write(ref state, Convert<ushort>(in value));
                    break;
                case TypeCode.UInt32:
                    UInt32Serializer.Instance.Write(ref state, Convert<uint>(in value));
                    break;
                case TypeCode.UInt64:
                    UInt64Serializer.Instance.Write(ref state, Convert<ulong>(in value));
                    break;
                default:
                    throw new InvalidOperationException("Failed to parse this type: " + Type.GetTypeCode(typeof(T)));
            }
        }

        private static T Convert<PRIM_TYPE>(PRIM_TYPE var)
        {
            Dictionary<PRIM_TYPE, T> dictionary = (Dictionary<PRIM_TYPE, T>)primitivesToType;
            if (!dictionary.TryGetValue(var, out T value))
            {
                object boxedVar = var;
                dictionary.Add(var, value = (T)boxedVar);
            }
            return value;
        }

        private static PRIM_TYPE Convert<PRIM_TYPE>(in T var)
        {
            Dictionary<T, PRIM_TYPE> dictionary = (Dictionary<T, PRIM_TYPE>)typeToPrimitives;
            if (!dictionary.TryGetValue(var, out PRIM_TYPE value))
            {
                object boxedVar = var;
                dictionary.Add(var, value = (PRIM_TYPE)boxedVar);
            }
            return value;
        }
        internal static void CreateType()
        { // Get the Type object for the constructed generic type (e.g., GenericClass<int>)
            Type constructedGenericType = typeof(EnumMemberSerializer<T>);

            // Define the parameter types for the desired constructor
            Type[] constructorParameterTypes = new Type[] { typeof(Type) };

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
                throw new InvalidOperationException("Failed to find a constructor for this type: " + typeof(T));
            }

            enumDecoratorConstructors.Add(typeof(T), constructor);

        }
    }
}