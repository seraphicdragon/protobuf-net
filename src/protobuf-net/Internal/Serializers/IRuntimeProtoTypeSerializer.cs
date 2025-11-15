using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using System;

namespace ProtoBuf.Internal.Serializers
{
    public interface IProtoTypeSerializer : IRuntimeProtoSerializerNode
    {
        Type BaseType { get; }
        internal bool HasCallbacks(TypeModel.CallbackType callbackType);
        bool CanCreateInstance();
        object CreateInstance(ISerializationContext context);
        internal void Callback(object value, TypeModel.CallbackType callbackType, ISerializationContext context);

        internal void EmitCallback(Compiler.CompilerContext ctx, Compiler.Local valueFrom, TypeModel.CallbackType callbackType);
        internal void EmitCreateInstance(Compiler.CompilerContext ctx, bool callNoteObject = true);
        bool ShouldEmitCreateInstance { get; }

        internal void EmitReadRoot(Compiler.CompilerContext ctx, Compiler.Local entity);
        internal void EmitWriteRoot(Compiler.CompilerContext ctx, Compiler.Local entity);

        bool HasInheritance { get; }

        bool IsSubType { get; }

        SerializerFeatures Features { get; }
    }
}