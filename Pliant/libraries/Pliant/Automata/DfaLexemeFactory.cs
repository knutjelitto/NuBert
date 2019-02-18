using System;
using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Automata
{
    public class DfaLexemeFactory : ILexemeFactory
    {
        public DfaLexemeFactory()
        {
            this._queue = new Queue<DfaLexeme>();
        }

        public LexerRuleType LexerRuleType => DfaLexerRule.DfaLexerRuleType;

        public ILexeme Create(LexerRule lexerRule, int position)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
            {
                throw new Exception(
                    $"Unable to create DfaLexeme from type {lexerRule.GetType().FullName}. Expected DfaLexerRule");
            }

            var dfaLexerRule = lexerRule as DfaLexerRule;
            if (this._queue.Count > 0)
            {
                var reusedLexeme = this._queue.Dequeue();
                reusedLexeme.Reset(dfaLexerRule, position);
                return reusedLexeme;
            }

            var dfaLexeme = new DfaLexeme(dfaLexerRule, position);
            return dfaLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var dfaLexeme = lexeme as DfaLexeme;
            if (dfaLexeme == null)
            {
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} with DfaLexemeFactory");
            }

            this._queue.Enqueue(dfaLexeme);
        }

        private readonly Queue<DfaLexeme> _queue;
    }
}