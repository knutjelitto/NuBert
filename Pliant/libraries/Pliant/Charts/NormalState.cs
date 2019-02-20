﻿using System.Text;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class NormalState : State
    {
        public NormalState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            this._hashCode = ComputeHashCode();
        }

        public NormalState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : this(dottedRule, origin)
        {
            ParseNode = parseNode;
        }

        public override bool Equals(object obj)
        {
            return obj is NormalState other && GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this._hashCode;
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
                    p == DottedRule.Position ? Dot : " ",
                    DottedRule.Production.RightHandSide[p]);
            }

            if (DottedRule.Position == DottedRule.Production.RightHandSide.Count)
            {
                stringBuilder.Append(Dot);
            }

            stringBuilder.Append($"\t\t({Origin})");
            return stringBuilder.ToString();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.Position.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.Production.GetHashCode());
        }

        private readonly int _hashCode;
    }
}