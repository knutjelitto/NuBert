using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Dotted
{
    public class DottedRuleAssortment
    {
        public DottedRuleAssortment(DottedRuleSet set)
        {
            this.set = set;
            this.reductions = new Dictionary<Symbol, DottedRuleAssortment>();
            this.tokenTransitions = new Dictionary<TokenType, DottedRuleAssortment>();
            this.scans = new Dictionary<Lexer, DottedRuleAssortment>();
            this.scanKeys = new List<Lexer>();

            this.hashCode = ComputeHashCode(set);
        }

        public IEnumerable<DottedRule> Data => this.set;

        public DottedRuleAssortment NullTransition { get; set; }

        public IReadOnlyDictionary<Symbol, DottedRuleAssortment> Reductions => this.reductions;

        public IReadOnlyList<Lexer> ScanKeys => this.scanKeys;

        public IReadOnlyDictionary<TokenType, DottedRuleAssortment> TokenTransitions => this.tokenTransitions;

        public void AddTransition(Symbol symbol, DottedRuleAssortment target)
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
            return obj is DottedRuleAssortment other && this.set.Equals(other.set);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private IReadOnlyDictionary<Lexer, DottedRuleAssortment> Scans => this.scans;

        private static int ComputeHashCode(DottedRuleSet data)
        {
            return data.GetHashCode();
        }

        private readonly int hashCode;
        private readonly Dictionary<Symbol, DottedRuleAssortment> reductions;
        private readonly List<Lexer> scanKeys;
        private readonly Dictionary<Lexer, DottedRuleAssortment> scans;
        private readonly DottedRuleSet set;
        private readonly Dictionary<TokenType, DottedRuleAssortment> tokenTransitions;
    }
}