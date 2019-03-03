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
            this.reductions = new Dictionary<Symbol, DottedRuleSet>();
            this.tokenTransitions = new Dictionary<TokenType, DottedRuleSet>();
            this.scans = new Dictionary<Lexer, DottedRuleSet>();
            this.scanKeys = new List<Lexer>();

            this.hashCode = ComputeHashCode(set);
        }

        public IEnumerable<DottedRule> Data => this.set;

        public DottedRuleSet NullTransition { get; set; }

        public IReadOnlyDictionary<Symbol, DottedRuleSet> Reductions => this.reductions;

        public IReadOnlyList<Lexer> ScanKeys => this.scanKeys;

        public IReadOnlyDictionary<TokenType, DottedRuleSet> TokenTransitions => this.tokenTransitions;

        public void AddTransition(Symbol symbol, DottedRuleSet target)
        {
            if (symbol is NonTerminal)
            {
                if (!Reductions.ContainsKey(symbol))
                {
                    this.reductions.Add(symbol, target);
                }
            }
            else if (symbol is Lexer lexerRule)
            {
                if (!Scans.ContainsKey(lexerRule))
                {
                    this.tokenTransitions.Add(lexerRule.TokenType, target);
                    this.scans.Add(lexerRule, target);
                    this.scanKeys.Add(lexerRule);
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

        private IReadOnlyDictionary<Lexer, DottedRuleSet> Scans => this.scans;

        private static int ComputeHashCode(IEnumerable<DottedRule> data)
        {
            return HashCode.Compute(data);
        }

        private readonly int hashCode;
        private readonly Dictionary<Symbol, DottedRuleSet> reductions;
        private readonly List<Lexer> scanKeys;
        private readonly Dictionary<Lexer, DottedRuleSet> scans;
        private readonly HashSet<DottedRule> set;
        private readonly Dictionary<TokenType, DottedRuleSet> tokenTransitions;
    }
}