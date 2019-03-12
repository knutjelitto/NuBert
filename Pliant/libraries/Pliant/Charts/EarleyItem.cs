using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public abstract class EarleyItem : ValueEqualityBase<EarleyItem>
    {
        protected EarleyItem(DottedRule dottedRule, int origin, IForestNode parseNode)
        {
            DottedRule = dottedRule;
            Origin = origin;
            ParseNode = parseNode;

            this.hashCode = (DottedRule, Origin).GetHashCode();
        }


        public DottedRule DottedRule { get; }
        public int Origin { get; }
        public IForestNode ParseNode { get; protected set; }
        public NonTerminal LeftHandSide => DottedRule.Production.LeftHandSide;

        protected override object ThisHashCode => (DottedRule, Origin);

        public abstract bool Enqueue(EarleySet set);

        public override string ToString()
        {
            return $"{DottedRule}\t\t({Origin})";
        }

        protected override bool ThisEquals(EarleyItem other)
        {
            return DottedRule.Equals(other.DottedRule) && Origin.Equals(other.Origin);
        }

        private readonly int hashCode;
    }
}