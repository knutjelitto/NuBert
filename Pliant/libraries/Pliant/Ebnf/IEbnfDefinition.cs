using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfDefinition : IEbnfNode
    {
        IEbnfBlock Block { get; }
    }

    public sealed class EbnfDefinitionSimple : ValueEqualityBase<EbnfDefinitionSimple>, IEbnfDefinition
    {
        public IEbnfBlock Block { get; }

        public EbnfDefinitionSimple(IEbnfBlock block)
        {
            Block = block;
        }

        protected override bool ThisEquals(EbnfDefinitionSimple other)
        {
            return other.Block.Equals(Block);
        }

        protected override object ThisHashCode => Block;
    }

    public sealed class EbnfDefinitionConcatenation : ValueEqualityBase<EbnfDefinitionConcatenation>, IEbnfDefinition
    {
        public EbnfDefinitionConcatenation(IEbnfBlock block, IEbnfDefinition definition)
        {
            Block = block;
            Definition = definition;
        }

        public IEbnfBlock Block { get; }
        public IEbnfDefinition Definition { get; }

        protected override bool ThisEquals(EbnfDefinitionConcatenation other)
        {
            return Block.Equals(other.Block) &&
                   Definition.Equals(other.Definition);
        }

        protected override object ThisHashCode => (Block, Definition);
    }
}