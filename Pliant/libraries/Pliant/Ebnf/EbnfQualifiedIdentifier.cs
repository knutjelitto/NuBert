using Pliant.Diagnostics;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public class EbnfQualifiedIdentifier : EbnfNode
    {
        private readonly int _hashCode;

        public string Identifier { get; private set; }

        public EbnfQualifiedIdentifier(string identifier)
        {
            Assert.IsNotNull(identifier, nameof(identifier));
            Identifier = identifier;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfQualifiedIdentifier;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var qualifiedIdentifier = obj as EbnfQualifiedIdentifier;
            if (qualifiedIdentifier == null)
            {
                return false;
            }

            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.Identifier.Equals(Identifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(NodeType.GetHashCode(), Identifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return Identifier;
        }
    }

    public class EbnfQualifiedIdentifierConcatenation : EbnfQualifiedIdentifier
    {
        private readonly int _hashCode;

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public EbnfQualifiedIdentifierConcatenation(
            string identifier,
            EbnfQualifiedIdentifier qualifiedIdentifier)
            : base(identifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfQualifiedIdentifierConcatenation;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var qualifiedIdentifier = obj as EbnfQualifiedIdentifierConcatenation;
            if (qualifiedIdentifier == null)
            {
                return false;
            }

            return qualifiedIdentifier.NodeType == NodeType
                && qualifiedIdentifier.Identifier.Equals(Identifier)
                && qualifiedIdentifier.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Identifier.GetHashCode(), 
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"{QualifiedIdentifier}.{Identifier}";
        }
    }
}