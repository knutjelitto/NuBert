using System.Collections.Generic;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfQualifiedIdentifier : EbnfNode
    {
        public EbnfQualifiedIdentifier(IEnumerable<string> identifiers)
            : this(identifiers.ToArray())
        {
        }

        public EbnfQualifiedIdentifier(params string[] identifiers)
        {
            Identifiers = identifiers;
            this.hashCode = HashCode.Compute(Identifiers);
        }

        public string[] Identifiers { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfQualifiedIdentifier other && Identifiers.SequenceEqual(other.Identifiers);
        }

        public override int GetHashCode() => this.hashCode;

        private readonly int hashCode;
    }

#if false
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
            EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier)
            : base(identifier)
        {
            QualifiedEbnfQualifiedIdentifier = qualifiedEbnfQualifiedIdentifier;
        }

        public EbnfQualifiedIdentifier QualifiedEbnfQualifiedIdentifier { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfQualifiedIdentifierConcatenation other &&
                   Identifier.Equals(other.Identifier) &&
                   QualifiedEbnfQualifiedIdentifier.Equals(other.QualifiedEbnfQualifiedIdentifier);
        }

        public override int GetHashCode()
        {
            return (Identifier, QualifiedIdentifier: QualifiedEbnfQualifiedIdentifier).GetHashCode();
        }

        public override string ToString()
        {
            return $"{QualifiedEbnfQualifiedIdentifier}.{Identifier}";
        }
    }
#endif
}