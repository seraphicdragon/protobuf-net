using System;
using System.Diagnostics;

namespace ProtoBuf.Internal.Serializers
{
    internal sealed class CharSerializer : UInt16Serializer, IRuntimeProtoSerializerNode<char>
    {
        private CharSerializer() : base() { }
        internal static new readonly CharSerializer Instance = new CharSerializer();

        private static readonly Type expectedType = typeof(char);

        public override Type ExpectedType => expectedType;

        public override void Write(ref ProtoWriter.State state, object value)
        {
            state.WriteUInt16((ushort)(char)value);
        }

        public override object Read(ref ProtoReader.State state, object value)
        {
            Debug.Assert(value is null); // since replaces
            return (char)state.ReadUInt16();
        }

        public void Write(ref ProtoWriter.State state, char value)
        {
            state.WriteUInt16(value);
        }

        public char Read(ref ProtoReader.State state, char value)
        {
            return (char)state.ReadUInt16();
        }

        // no need for any special IL here; ushort and char are
        // interchangeable as long as there is no boxing/unboxing
    }
}