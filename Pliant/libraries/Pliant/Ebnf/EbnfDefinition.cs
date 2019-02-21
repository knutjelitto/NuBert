namespace Pliant.Ebnf
{
    public abstract class EbnfDefinition : EbnfNode
    {
        protected EbnfDefinition(EbnfBlock block)
        {
            Block = block;
        }

        public EbnfBlock Block { get; }
    }

    public sealed class EbnfDefinitionSimple : EbnfDefinition
    {
        public EbnfDefinitionSimple(EbnfBlock block)
            : base(block)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfDefinitionSimple other && 
                   other.Block.Equals(Block);
        }

        public override int GetHashCode()
        {
            return Block.GetHashCode();
        }
    }

    public sealed class EbnfDefinitionConcatenation : EbnfDefinition
    {
        public EbnfDefinitionConcatenation(EbnfBlock block, EbnfDefinition definition)
            : base(block)
        {
            Definition = definition;
        }

        public EbnfDefinition Definition { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfDefinitionConcatenation other && 
                   Block.Equals(other.Block) && 
                   Definition.Equals(other.Definition);
        }

        public override int GetHashCode()
        {
            return (Block, Definition).GetHashCode();
        }
    }
}