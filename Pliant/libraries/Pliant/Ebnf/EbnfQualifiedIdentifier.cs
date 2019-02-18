namespace Pliant.Ebnf
{
    public class EbnfQualifiedIdentifier : EbnfNode
    {
        public EbnfQualifiedIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfQualifiedIdentifier other &&
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

    public class EbnfQualifiedIdentifierConcatenation : EbnfQualifiedIdentifier
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
                   other.Identifier.Equals(Identifier) &&
                   other.QualifiedIdentifier.Equals(QualifiedIdentifier);
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