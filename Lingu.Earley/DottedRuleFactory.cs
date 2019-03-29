using System;
using System.Collections.Generic;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class DottedRuleFactory
    {
        public DottedRuleFactory(IEnumerable<Production> productions)
        {
            foreach (var production in productions)
            {
                var rules = new DottedRule[production.Count + 1];
                this.index.Add(production, rules);
                for (var dot = 0; dot <= production.Count; dot++)
                {
                    rules[dot] = new DottedRule(this, production, dot);
                }
            }
        }

        public DottedRule Get(Production production, int dot)
        {
            if (dot >= 0 && dot <= production.Count && this.index.TryGetValue(production, out var dotted) && dot < dotted.Length)
            {
                return dotted[dot];
            }

            throw new ArgumentOutOfRangeException(nameof(dot));
        }

        private readonly Dictionary<Production, DottedRule[]> index = new Dictionary<Production, DottedRule[]>();
    }
}