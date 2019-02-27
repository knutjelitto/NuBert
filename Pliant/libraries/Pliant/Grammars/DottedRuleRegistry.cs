using System.Collections.Generic;

namespace Pliant.Grammars
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
            if (dot >= 0 && this.index.TryGetValue(production, out var rules) && dot < rules.Length)
            {
                return rules[dot];
            }

            return null;
        }

        public DottedRule GetNext(DottedRule dottedRule)
        {
            return Get(dottedRule.Production, dottedRule.Dot + 1);
        }

        private void Seed(IEnumerable<Production> productions)
        {
            foreach (var production in productions)
            {
                var rules = new DottedRule[production.RightHandSide.Count + 1];
                this.index.Add(production, rules);
                for (var dot = 0; dot <= production.RightHandSide.Count; dot++)
                {
                    rules[dot] = new DottedRule(production, dot);
                }
            }
        }

        private readonly Dictionary<Production, DottedRule[]> index;
    }
}