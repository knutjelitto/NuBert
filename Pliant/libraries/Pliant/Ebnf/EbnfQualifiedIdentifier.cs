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
            : base(HashCode.Compute(identifiers))
        {
            Identifiers = identifiers;
        }

        public string[] Identifiers { get; }

        public override bool ThisEquals(EbnfQualifiedIdentifier other)
        {
            return Identifiers.SequenceEqual(other.Identifiers);
        }
    }
}