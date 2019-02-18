using System.Text;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class NormalState : State
    {
        public NormalState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
        }

        public NormalState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : this(dottedRule, origin)
        {
            ParseNode = parseNode;
        }

        public override StateType StateType => StateType.Normal;

        public override bool Equals(object obj)
        {
            return obj is NormalState other &&
                   DottedRule.Equals(other.DottedRule) &&
                   Origin.Equals(other.Origin);
        }

        public override int GetHashCode()
        {
            return (DottedRule, Origin).GetHashCode();
        }

        public bool IsSource(Symbol searchSymbol)
        {
            var dottedRule = DottedRule;
            if (dottedRule.IsComplete)
            {
                return false;
            }

            return dottedRule.PostDotSymbol.Equals(searchSymbol);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .AppendFormat("{0} ->", DottedRule.Production.LeftHandSide.Value);
            const string Dot = "\u25CF";

            for (var p = 0; p < DottedRule.Production.RightHandSide.Count; p++)
            {
                stringBuilder.AppendFormat(
                    "{0}{1}",
                    p == DottedRule.Dot ? Dot : " ",
                    DottedRule.Production.RightHandSide[p]);
            }

            if (DottedRule.Dot == DottedRule.Production.RightHandSide.Count)
            {
                stringBuilder.Append(Dot);
            }

            stringBuilder.Append($"\t\t({Origin})");
            return stringBuilder.ToString();
        }
    }
}