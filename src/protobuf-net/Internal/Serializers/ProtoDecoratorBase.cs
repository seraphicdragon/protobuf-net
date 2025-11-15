using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Internal.Serializers
{
    internal abstract class ProtoDecoratorBase : IRuntimeProtoSerializerNode
    {
        private IRuntimeProtoSerializerNode endTail = null;
        private readonly ValueMember valueMember;
        public virtual bool IsScalar => Tail.IsScalar;
        public abstract Type ExpectedType { get; }
        protected readonly IRuntimeProtoSerializerNode Tail;

        public IRuntimeProtoSerializerNode EndTail
        {
            get
            {
                if(endTail == null)
                {
                    IRuntimeProtoSerializerNode currentTail = Tail;
                    while (currentTail != null)
                    {
                        endTail = currentTail;
                        ProtoDecoratorBase decorator = currentTail as ProtoDecoratorBase;
                        if (decorator == null)
                            break;
                        
                        currentTail = decorator.Tail;
                    }
                }
                return endTail;
            }
        }
        protected ProtoDecoratorBase(ValueMember valueMember, IRuntimeProtoSerializerNode tail)
        {
            this.valueMember = valueMember;
            this.Tail = tail;
        }
        public abstract bool ReturnsValue { get; }
        public abstract bool RequiresOldValue { get; }
        public abstract void Write(ref ProtoWriter.State state, object value);
        public abstract object Read(ref ProtoReader.State state, object value);

        void IRuntimeProtoSerializerNode.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom) { EmitWrite(ctx, valueFrom); }
        protected abstract void EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom);
        void IRuntimeProtoSerializerNode.EmitRead(Compiler.CompilerContext ctx, Compiler.Local entity) { EmitRead(ctx, entity); }
        protected abstract void EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom);

        public ValueMember ValueMember { get { return valueMember; } }
    }
}