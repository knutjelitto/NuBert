﻿using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleRegistry
    {
        private readonly Dictionary<Production, Dictionary<int, DottedRule>> _dottedRuleIndex;

        public DottedRuleRegistry()
        {
            this._dottedRuleIndex = new Dictionary<Production, Dictionary<int, DottedRule>>(
                new HashCodeEqualityComparer<Production>());
        }

        public void Seed(IGrammar grammar)
        {
            var ruleId = 0;

            foreach (var production in grammar.Productions)
            {
                for (var dot = 0; dot <= production.RightHandSide.Count; dot++)
                {
                    var dottedRule = new DottedRule(ruleId++, production, dot);
                    Register(dottedRule);
                }
            }
        }

        public void Register(DottedRule dottedRule)
        {
            var positionIndex = this._dottedRuleIndex.AddOrGetExisting(dottedRule.Production);
            positionIndex[dottedRule.Position] = dottedRule;
        }

        public DottedRule Get(Production production, int position)
        {
            if (!this._dottedRuleIndex.TryGetValue(production, out var positionIndex))
            {
                return null;
            }

            if (!positionIndex.TryGetValue(position, out var dottedRule))
            {
                return null;
            }

            return dottedRule;
        }

        public DottedRule GetNext(DottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Position + 1);
        }        
    }
}
