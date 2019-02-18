namespace Pliant.Ebnf
{
    public class EbnfDefinition : EbnfNode
    {
        public EbnfDefinition(EbnfBlock block)
        {
            Block = block;
        }

        public EbnfBlock Block { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfDefinition other && 
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
                   other.Block.Equals(Block) && 
                   other.Definition.Equals(Definition);
        }

        public override int GetHashCode()
        {
            return (Block, Definition).GetHashCode();
        }
    }
}