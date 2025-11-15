using ProtoBuf.Compiler;
using System;

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

        public void Write(ref ProtoWriter.State state, sbyte value)
        {
            SByteSerializer.Instance.Write(ref state, value);
        }

        public sbyte Read(ref ProtoReader.State state, sbyte value)
        {
            return SByteSerializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, short value)
        {
            Int16Serializer.Instance.Write(ref state, value);
        }

        public short Read(ref ProtoReader.State state, short value)
        {
            return Int16Serializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, int value)
        {
            Int16Serializer.Instance.Write(ref state, value);
        }

        public int Read(ref ProtoReader.State state, int value)
        {
            return Int32Serializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, long value)
        {
            Int64Serializer.Instance.Write(ref state, value);
        }

        public long Read(ref ProtoReader.State state, long value)
        {
            return Int64Serializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, byte value)
        {
            ByteSerializer.Instance.Write(ref state, value);
        }

        public byte Read(ref ProtoReader.State state, byte value)
        {
            return ByteSerializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, ushort value)
        {
            UInt16Serializer.Instance.Write(ref state, value);
        }

        public ushort Read(ref ProtoReader.State state, ushort value)
        {
            return UInt16Serializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, uint value)
        {
            UInt32Serializer.Instance.Write(ref state, value);
        }

        public uint Read(ref ProtoReader.State state, uint value)
        {
            return UInt32Serializer.Instance.Read(ref state, value);
        }

        public void Write(ref ProtoWriter.State state, ulong value)
        {
            UInt64Serializer.Instance.Write(ref state, value);
        }

        public ulong Read(ref ProtoReader.State state, ulong value)
        {
            return UInt64Serializer.Instance.Read(ref state, value);
        }
    }
}