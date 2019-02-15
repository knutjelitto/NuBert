using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    public class DottedRuleSet
    {
        private readonly IDottedRule[] _cachedData;        
        private readonly SortedSet<IDottedRule> _set;
        private readonly Dictionary<ISymbol, DottedRuleSet> _reductions;
        private readonly Dictionary<TokenType, DottedRuleSet> _tokenTransitions;
        private readonly Dictionary<ILexerRule, DottedRuleSet> _scans;
        private readonly List<ILexerRule> _scanKeys;

        public IReadOnlyList<IDottedRule> Data => this._cachedData;

        public IReadOnlyDictionary<ISymbol, DottedRuleSet> Reductions => this._reductions;

        public IReadOnlyDictionary<TokenType, DottedRuleSet> TokenTransitions => this._tokenTransitions;

        public IReadOnlyDictionary<ILexerRule, DottedRuleSet> Scans => this._scans;

        public IReadOnlyList<ILexerRule> ScanKeys => this._scanKeys;

        public DottedRuleSet NullTransition { get; set; }

        public DottedRuleSet(SortedSet<IDottedRule> set)
        {
            this._set = set;
            this._cachedData = this._set.ToArray();
            this._reductions = new Dictionary<ISymbol, DottedRuleSet>();
            this._tokenTransitions = new Dictionary<TokenType, DottedRuleSet>();
            this._scans = new Dictionary<ILexerRule, DottedRuleSet>();
            this._scanKeys = new List<ILexerRule>();

            this._hashCode = ComputeHashCode(set);
        }

        private readonly int _hashCode;

        public void AddTransistion(ISymbol symbol, DottedRuleSet target)
        {
            if (symbol.SymbolType == SymbolType.NonTerminal)
            {
                if (!Reductions.ContainsKey(symbol))
                {
                    this._reductions.Add(symbol, target);
                }
            }
            else if(symbol.SymbolType == SymbolType.LexerRule)
            {
                var lexerRule = symbol as ILexerRule;
                if (!Scans.ContainsKey(lexerRule))
                {
                    this._tokenTransitions.Add(lexerRule.TokenType, target);
                    this._scans.Add(lexerRule, target);
                    this._scanKeys.Add(lexerRule);
                }
            }
        }

        public bool Contains(IDottedRule state)
        {
            return this._set.Contains(state);
        }

        static int ComputeHashCode(SortedSet<IDottedRule> data)
        {
            return HashCode.Compute(data);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var dottedRuleSet = obj as DottedRuleSet;
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
    }
}
