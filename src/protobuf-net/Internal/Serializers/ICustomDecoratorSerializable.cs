using ProtoBuf.Internal.Serializers;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
    public interface ICustomDecoratorSerializable
    {
        bool TrySerializeMember(ValueMember valueMember);
        bool TryWrite(ref ProtoWriter.State state, ValueMember valueMember, IRuntimeProtoSerializerNode tail);
        bool TryRead(ref ProtoReader.State state, ValueMember valueMember, IRuntimeProtoSerializerNode endTail);
        bool IsDefault(ValueMember valueMember);
    }
}
