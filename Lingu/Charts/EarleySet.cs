using System.Collections.Generic;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public class EarleySet
    {
        public EarleySet()
        {
            this.completions = new EarleyItemList<CompletedItem>();
            this.nonterminals = new EarleyItemList<NonterminalItem>();
            this.terminals = new EarleyItemList<TerminalItem>();
        }

        public IReadOnlyList<CompletedItem> Completions => this.completions;

        public IReadOnlyList<NonterminalItem> Nonterminals => this.nonterminals;

        public IReadOnlyList<TerminalItem> Terminals => this.terminals;

        public bool Add(CompletedItem item)
        {
            return this.completions.Add(item);
        }

        public bool Add(NonterminalItem item)
        {
            return this.nonterminals.Add(item);
        }

        public bool Add(TerminalItem item)
        {
            return this.terminals.Add(item);
        }

        public bool Contains(DottedRule dottedRule, int origin)
        {
            if (dottedRule.IsComplete)
            {
                return CompletionsContains(dottedRule, origin);
            }

            var currentSymbol = dottedRule.PostDot;
            return currentSymbol is Nonterminal
                       ? NonterminalsContains(dottedRule, origin)
                       : TerminalsContains(dottedRule, origin);
        }

        private bool CompletionsContains(DottedRule rule, int origin)
        {
            return this.completions.Contains(rule, origin);
        }

        private bool NonterminalsContains(DottedRule rule, int origin)
        {
            return this.nonterminals.Contains(rule, origin);
        }

        private bool TerminalsContains(DottedRule rule, int origin)
        {
            return this.terminals.Contains(rule, origin);
        }

        private readonly EarleyItemList<CompletedItem> completions;
        private readonly EarleyItemList<NonterminalItem> nonterminals;
        private readonly EarleyItemList<TerminalItem> terminals;
    }
}
