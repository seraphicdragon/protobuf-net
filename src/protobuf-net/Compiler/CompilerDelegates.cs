using ProtoBuf.Serializers;

namespace ProtoBuf.Compiler
{
    public delegate void ProtoSerializer<T>(ref ProtoWriter.State state, T value);
    public delegate T ProtoDeserializer<T>(ref ProtoReader.State state, T value);
    public delegate T ProtoSubTypeDeserializer<T>(ref ProtoReader.State state, SubTypeState<T> value) where T : class;
}