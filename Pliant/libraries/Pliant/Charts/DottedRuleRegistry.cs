using System;
using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class DottedRuleRegistry
    {
        public DottedRuleRegistry(IEnumerable<Production> productions)
        {
            this.index = new Dictionary<Production, DottedRule[]>();
            Seed(productions);
        }

        public DottedRule Get(Production production, int dot)
        {
            if (dot >= 0 && dot <= production.Count && this.index.TryGetValue(production, out var rules) && dot < rules.Length)
            {
                return rules[dot];
            }

            throw new ArgumentOutOfRangeException(nameof(dot));
        }

        public DottedRule GetNext(DottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Dot + 1);
        }

        private void Seed(IEnumerable<Production> productions)
        {
            foreach (var production in productions)
            {
                var rules = new DottedRule[production.Count + 1];
                this.index.Add(production, rules);
                for (var dot = 0; dot <= production.Count; dot++)
                {
                    rules[dot] = new DottedRule(production, dot);
                }
            }
        }

        private readonly Dictionary<Production, DottedRule[]> index;
    }
}