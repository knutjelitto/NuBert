using System.Collections.Generic;
using System.Linq;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class EarleySet
    {
        public EarleySet()
        {
            this.completionStates = new EarleyStateList<CompletedState>();
            this.nonterminalStates = new EarleyStateList<NonterminalState>();
            this.terminalStates = new EarleyStateList<TerminalState>();
        }

        public IReadOnlyList<CompletedState> CompletionStates => this.completionStates;

        public IReadOnlyList<NonterminalState> NonterminalStates => this.nonterminalStates;

        public IReadOnlyList<TerminalState> TerminalStates => this.terminalStates;

        public IEnumerable<Terminal> Terminals => TerminalStates.Select(state => state.DottedRule.PostDot).Cast<Terminal>();

        public bool Add(CompletedState item)
        {
            return this.completionStates.Add(item);
        }

        public bool Add(NonterminalState state)
        {
            return this.nonterminalStates.Add(state);
        }

        public bool Add(TerminalState item)
        {
            return this.terminalStates.Add(item);
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
            return this.completionStates.Contains(rule, origin);
        }

        private bool NonterminalsContains(DottedRule rule, int origin)
        {
            return this.nonterminalStates.Contains(rule, origin);
        }

        private bool TerminalsContains(DottedRule rule, int origin)
        {
            return this.terminalStates.Contains(rule, origin);
        }

        private readonly EarleyStateList<CompletedState> completionStates;
        private readonly EarleyStateList<NonterminalState> nonterminalStates;
        private readonly EarleyStateList<TerminalState> terminalStates;
    }
}
