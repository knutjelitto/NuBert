using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry //: IDottedRuleRegistry
    {
        private readonly Dictionary<Production, Dictionary<int, DottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            this._dottedRuleIndex = new Dictionary<Production, Dictionary<int, DottedRule>>(
                new HashCodeEqualityComparer<Production>());
        }

        public void SeedFrom(IGrammar grammar)
        {
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                for (var s = 0; s <= production.RightHandSide.Count; s++)
                {
                    var dottedRule = new DottedRule(production, s);
                    Register(dottedRule);
                }
            }
        }

        public void Register(DottedRule dottedRule)
        {
            var positionIndex = this._dottedRuleIndex.AddOrGetExisting(dottedRule.Production);
            positionIndex[dottedRule.Dot] = dottedRule;
        }

        public DottedRule Get(Production production, int dot)
        {
            if (!this._dottedRuleIndex.TryGetValue(production, out var positionIndex))
            {
                return null;
            }

            if (!positionIndex.TryGetValue(dot, out var dottedRule))
            {
                return null;
            }

            return dottedRule;
        }

        public DottedRule GetNext(DottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Dot + 1);
        }        
    }
}
