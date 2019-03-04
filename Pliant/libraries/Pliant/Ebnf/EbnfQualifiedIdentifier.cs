using System.Collections.Generic;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public sealed class EbnfQualifiedIdentifier : ValueEqualityBase<EbnfQualifiedIdentifier>, IEbnfNode
    {
        public EbnfQualifiedIdentifier(IEnumerable<string> identifiers)
            : this(identifiers.ToArray())
        {
        }

        public EbnfQualifiedIdentifier(params string[] identifiers)
        {
            Identifiers = identifiers;
        }

        public string[] Identifiers { get; }

        protected override bool ThisEquals(EbnfQualifiedIdentifier other)
        {
            return Identifiers.SequenceEqual(other.Identifiers);
        }

        protected override object ThisHashCode => HashCode.Compute(Identifiers);
    }
}