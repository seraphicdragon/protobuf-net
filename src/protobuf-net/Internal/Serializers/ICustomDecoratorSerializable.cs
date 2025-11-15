using ProtoBuf.Internal.Serializers;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
    public interface ICustomDecoratorSerializable
    {
        bool CanWrite(ValueMember valueMember);
        bool TryWrite(ref ProtoWriter.State state, ValueMember valueMember, IRuntimeProtoSerializerNode tail);
    }
}
