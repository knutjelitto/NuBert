using System.Collections.Generic;
using System.Linq;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleAssortment
    {
        public DottedRuleAssortment(DottedRuleSet set)
        {
            this._set = set;
            this._cachedData = this._set.ToArray();
            this._reductions = new Dictionary<Symbol, DottedRuleAssortment>();
            this._tokenTransitions = new Dictionary<TokenType, DottedRuleAssortment>();
            this._scans = new Dictionary<LexerRule, DottedRuleAssortment>();
            this._scanKeys = new List<LexerRule>();

            this._hashCode = ComputeHashCode(set);
        }

        public IReadOnlyList<DottedRule> Data => this._cachedData;

        public DottedRuleAssortment NullTransition { get; set; }

        public IReadOnlyDictionary<Symbol, DottedRuleAssortment> Reductions => this._reductions;

        public IReadOnlyList<LexerRule> ScanKeys => this._scanKeys;

        public IReadOnlyDictionary<LexerRule, DottedRuleAssortment> Scans => this._scans;

        public IReadOnlyDictionary<TokenType, DottedRuleAssortment> TokenTransitions => this._tokenTransitions;

        public void AddTransistion(Symbol symbol, DottedRuleAssortment target)
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
            if (obj == null)
            {
                return false;
            }

            var dottedRuleSet = obj as DottedRuleAssortment;
            if (dottedRuleSet == null)
            {
                return false;
            }

            foreach (var item in this._cachedData)
            {
                if (!dottedRuleSet.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private static int ComputeHashCode(DottedRuleSet data)
        {
            return HashCode.Compute(data);
        }

        private readonly DottedRule[] _cachedData;

        private readonly int _hashCode;
        private readonly Dictionary<Symbol, DottedRuleAssortment> _reductions;
        private readonly List<LexerRule> _scanKeys;
        private readonly Dictionary<LexerRule, DottedRuleAssortment> _scans;
        private readonly DottedRuleSet _set;
        private readonly Dictionary<TokenType, DottedRuleAssortment> _tokenTransitions;
    }
}