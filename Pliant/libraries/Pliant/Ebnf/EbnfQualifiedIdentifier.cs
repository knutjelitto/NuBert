namespace Pliant.Ebnf
{
    public abstract class EbnfQualifiedIdentifier : EbnfNode
    {
        protected EbnfQualifiedIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }
    }

    public sealed class EbnfQualifiedIdentifierSimple : EbnfQualifiedIdentifier
    {
        public EbnfQualifiedIdentifierSimple(string identifier)
            : base(identifier)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfQualifiedIdentifierSimple other &&
                   other.Identifier.Equals(Identifier);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return Identifier;
        }
    }

    public sealed class EbnfQualifiedIdentifierConcatenation : EbnfQualifiedIdentifier
    {
        public EbnfQualifiedIdentifierConcatenation(
            string identifier,
            EbnfQualifiedIdentifier qualifiedIdentifier)
            : base(identifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfQualifiedIdentifierConcatenation other &&
                   Identifier.Equals(other.Identifier) &&
                   QualifiedIdentifier.Equals(other.QualifiedIdentifier);
        }

        public override int GetHashCode()
        {
            return (Identifier, QualifiedIdentifier).GetHashCode();
        }

        public override string ToString()
        {
            return $"{QualifiedIdentifier}.{Identifier}";
        }
    }
}