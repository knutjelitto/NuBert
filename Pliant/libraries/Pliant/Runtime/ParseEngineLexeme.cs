﻿using Pliant.Grammars;

using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Runtime
{
    public class ParseEngineLexeme : LexemeBase<IGrammarLexerRule>, ILexeme
    {
        public override string Value => this._capture.ToString();

        private readonly StringBuilder _capture;
        private IParseEngine _parseEngine;

        public ParseEngineLexeme(IGrammarLexerRule lexerRule)
            : base(lexerRule, 0)
        {
            this._capture = new StringBuilder();
            this._parseEngine = new ParseEngine(lexerRule.Grammar);
        }

        public override bool Scan(char c)
        {
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = SharedPools.Default<List<TerminalLexeme>>().AllocateAndClear();
            var expectedLexerRules = this._parseEngine.GetExpectedLexerRules();

            foreach (var rule in expectedLexerRules)
            {
                if (rule.LexerRuleType == TerminalLexerRule.TerminalLexerRuleType)
                {
                    expectedLexemes.Add(new TerminalLexeme(rule as ITerminalLexerRule, Position));
                }
            }

            // filter on first rule to pass (since all rules are one character per lexeme)
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            TerminalLexeme firstPassingRule = null;
            foreach (var lexeme in expectedLexemes)
            {
                if (lexeme.Scan(c))
                {
                    firstPassingRule = lexeme;
                    break;
                }
            }

            SharedPools.Default<List<TerminalLexeme>>()
                .ClearAndFree(expectedLexemes);

            if (firstPassingRule == null)
            {
                return false;
            }

            var result = this._parseEngine.Pulse(firstPassingRule);
            if (result)
            {
                this._capture.Append(c);
            }

            return result;
        }

        public override bool IsAccepted()
        {
            return this._parseEngine.IsAccepted();
        }

        public override void Reset()
        {
            this._capture.Clear();
            this._parseEngine = new ParseEngine(ConcreteLexerRule.Grammar);
        }
    }
}