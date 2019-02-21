﻿using System.Collections.Generic;
using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class DottedRuleAssortment
    {
        public DottedRuleAssortment(DottedRuleSet set)
        {
            this._set = set;
            this._reductions = new Dictionary<Symbol, DottedRuleAssortment>();
            this._tokenTransitions = new Dictionary<TokenType, DottedRuleAssortment>();
            this._scans = new Dictionary<LexerRule, DottedRuleAssortment>();
            this._scanKeys = new List<LexerRule>();

            this._hashCode = ComputeHashCode(set);
        }

        public IEnumerable<DottedRule> Data => this._set;

        public DottedRuleAssortment NullTransition { get; set; }

        public IReadOnlyDictionary<Symbol, DottedRuleAssortment> Reductions => this._reductions;

        public IReadOnlyList<LexerRule> ScanKeys => this._scanKeys;

        public IReadOnlyDictionary<TokenType, DottedRuleAssortment> TokenTransitions => this._tokenTransitions;

        public void AddTransition(Symbol symbol, DottedRuleAssortment target)
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
            return obj is DottedRuleAssortment other && this._set.Equals(other._set);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private IReadOnlyDictionary<LexerRule, DottedRuleAssortment> Scans => this._scans;

        private static int ComputeHashCode(DottedRuleSet data)
        {
            return data.GetHashCode();
        }

        private readonly int _hashCode;
        private readonly Dictionary<Symbol, DottedRuleAssortment> _reductions;
        private readonly List<LexerRule> _scanKeys;
        private readonly Dictionary<LexerRule, DottedRuleAssortment> _scans;
        private readonly DottedRuleSet _set;
        private readonly Dictionary<TokenType, DottedRuleAssortment> _tokenTransitions;
    }
}