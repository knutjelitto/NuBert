using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfDefinition : EbnfNode
    {
        private readonly int _hashCode;

        public EbnfBlock Block { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfDefinition;

        public EbnfDefinition(EbnfBlock block)
        {
            Assert.IsNotNull(block, nameof(block));
            Block = block;
            this._hashCode = ComputeHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var ebnfDefinition = obj as EbnfDefinition;
            if (ebnfDefinition == null)
            {
                return false;
            }

            return ebnfDefinition.NodeType == NodeType 
                && ebnfDefinition.Block.Equals(Block);
        }
        
        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Block.GetHashCode(), 
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }

    public class EbnfDefinitionConcatenation : EbnfDefinition
    {
        private readonly int _hashCode;

        public EbnfDefinition Definition { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfDefinitionConcatenation;

        public EbnfDefinitionConcatenation(EbnfBlock block, EbnfDefinition definition)
            : base(block)
        {
            Assert.IsNotNull(definition, nameof(definition));
            Definition = definition;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var ebnfDefinitionConcatenation = obj as EbnfDefinitionConcatenation;
            if (ebnfDefinitionConcatenation == null)
            {
                return false;
            }

            return ebnfDefinitionConcatenation.NodeType == EbnfNodeType.EbnfDefinitionConcatenation
                && ebnfDefinitionConcatenation.Block.Equals(Block)
                && ebnfDefinitionConcatenation.Definition.Equals(Definition);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                Block.GetHashCode(),
                Definition.GetHashCode(),
                NodeType.GetHashCode());
        }
        
        public override int GetHashCode()
        {
            return this._hashCode;
        }
    }
}