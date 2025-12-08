using ProtoBuf.Internal;
using ProtoBuf.Serializers;
using ProtoBuf.WellKnownTypes;
using System.Runtime.InteropServices;

namespace ProtoBuf.WellKnownTypes
{
    /// <summary>
    /// A generic empty message that you can re-use to avoid defining duplicated empty messages in your APIs
    /// </summary>
    [ProtoContract(Name = ".google.protobuf.Empty", Serializer = typeof(PrimaryTypeProvider), Origin = "google/protobuf/empty.proto")]
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public readonly struct Empty
    {
    }
}