using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;

namespace Pliant.Runtime
{
    public class ParseEngineLexemeFactory : ILexemeFactory
    {
        private readonly Queue<ParseEngineLexeme> _queue;

        public LexerRuleType LexerRuleType => GrammarLexerRule.GrammarLexerRuleType;

        public ParseEngineLexemeFactory()
        {
            this._queue = new Queue<ParseEngineLexeme>();
        }

        public ILexeme Create(ILexerRule lexerRule, int position)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
            {
                throw new Exception(
                    $"Unable to create ParseEngineLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");
            }

            var grammarLexerRule = lexerRule as IGrammarLexerRule;

            if (this._queue.Count == 0)
            {
                return new ParseEngineLexeme(grammarLexerRule);
            }

            var reusedLexeme = this._queue.Dequeue();
            reusedLexeme.Reset(grammarLexerRule, position);
            return reusedLexeme;
        }

        public void Free(ILexeme lexeme)
        {
            var parseEngineLexeme = lexeme as ParseEngineLexeme;
            if(parseEngineLexeme == null)
            {
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from ParseEngineLexeme.");
            }

            this._queue.Enqueue(parseEngineLexeme);
        }
    }
}