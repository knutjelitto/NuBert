using System.Collections.Generic;
using System.Linq;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleSet
    {
        public DottedRuleSet(DottedSet set)
        {
            this._set = set;
            this._cachedData = this._set.ToArray();
            this._reductions = new Dictionary<Symbol, DottedRuleSet>();
            this._tokenTransitions = new Dictionary<TokenType, DottedRuleSet>();
            this._scans = new Dictionary<LexerRule, DottedRuleSet>();
            this._scanKeys = new List<LexerRule>();

            this._hashCode = ComputeHashCode(set);
        }

        public IReadOnlyList<DottedRule> Data => this._cachedData;

        public DottedRuleSet NullTransition { get; set; }

        public IReadOnlyDictionary<Symbol, DottedRuleSet> Reductions => this._reductions;

        public IReadOnlyList<LexerRule> ScanKeys => this._scanKeys;

        public IReadOnlyDictionary<LexerRule, DottedRuleSet> Scans => this._scans;

        public IReadOnlyDictionary<TokenType, DottedRuleSet> TokenTransitions => this._tokenTransitions;

        public void AddTransistion(Symbol symbol, DottedRuleSet target)
        {
            if (symbol is NonTerminal)
            {
                if (!Reductions.ContainsKey(symbol))
                {
                    this._reductions.Add(symbol, target);
                }
            }
            else if (symbol is LexerRule lexerRule)
            {
                if (!Scans.ContainsKey(lexerRule))
                {
                    this._tokenTransitions.Add(lexerRule.TokenType, target);
                    this._scans.Add(lexerRule, target);
                    this._scanKeys.Add(lexerRule);
                }
            }
        }

        public bool Contains(DottedRule state)
        {
            return this._set.Contains(state);
        }

        public override bool Equals(object obj)
        {
            return obj is DottedRuleSet other && this._cachedData.All(item => other.Contains(item));
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private static int ComputeHashCode(DottedSet data)
        {
            return HashCode.Compute(data);
        }

        private readonly DottedRule[] _cachedData;

        private readonly int _hashCode;
        private readonly Dictionary<Symbol, DottedRuleSet> _reductions;
        private readonly List<LexerRule> _scanKeys;
        private readonly Dictionary<LexerRule, DottedRuleSet> _scans;
        private readonly DottedSet _set;
        private readonly Dictionary<TokenType, DottedRuleSet> _tokenTransitions;
    }
}