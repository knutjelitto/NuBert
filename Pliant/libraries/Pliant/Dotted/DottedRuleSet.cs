using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Dotted
{
    public class DottedRuleSet
    {
        public DottedRuleSet(HashSet<DottedRule> set)
        {
            this.set = set;
            this.reductions = new Dictionary<NonTerminal, DottedRuleSet>();
            this.tokenTransitions = new Dictionary<TokenClass, DottedRuleSet>();
            this.scans = new Dictionary<LexerRule, DottedRuleSet>();
            this.scanKeys = new List<LexerRule>();

            this.hashCode = ComputeHashCode(set);
        }

        public IEnumerable<DottedRule> Data => this.set;

        public DottedRuleSet NullTransition { get; set; }

        public IReadOnlyDictionary<NonTerminal, DottedRuleSet> Reductions => this.reductions;

        public IReadOnlyList<LexerRule> ScanKeys => this.scanKeys;

        public IReadOnlyDictionary<TokenClass, DottedRuleSet> TokenTransitions => this.tokenTransitions;

        public void AddTransition(Symbol symbol, DottedRuleSet target)
        {
            if (symbol is NonTerminal nonTerminal)
            {
                if (!Reductions.ContainsKey(nonTerminal))
                {
                    this.reductions.Add(nonTerminal, target);
                }
            }
            else if (symbol is LexerRule lexer)
            {
                if (!this.scans.ContainsKey(lexer))
                {
                    this.tokenTransitions.Add(lexer.TokenClass, target);
                    this.scans.Add(lexer, target);
                    this.scanKeys.Add(lexer);
                }
            }
        }

        public bool Contains(DottedRule state)
        {
            return this.set.Contains(state);
        }

        public override bool Equals(object obj)
        {
            return obj is DottedRuleSet other && this.set.SetEquals(other.set);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private static int ComputeHashCode(IEnumerable<DottedRule> data)
        {
            return HashCode.Compute(data);
        }

        private readonly int hashCode;
        private readonly Dictionary<NonTerminal, DottedRuleSet> reductions;
        private readonly List<LexerRule> scanKeys;
        private readonly Dictionary<LexerRule, DottedRuleSet> scans;
        private readonly HashSet<DottedRule> set;
        private readonly Dictionary<TokenClass, DottedRuleSet> tokenTransitions;
    }
}