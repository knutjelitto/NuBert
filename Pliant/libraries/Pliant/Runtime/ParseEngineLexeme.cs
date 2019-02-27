using System.Collections.Generic;
using System.Text;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class ParseEngineLexeme : Lexeme
    {
        public ParseEngineLexeme(GrammarLexerRule lexerRule)
            : base(lexerRule, 0)
        {
            this.capture = new StringBuilder();
            this.parseEngine = new ParseEngine(lexerRule.Grammar);
        }

        public override string Value => this.capture.ToString();

        public override bool IsAccepted()
        {
            return this.parseEngine.IsAccepted();
        }

        public override bool Scan(char c)
        {
            var pool = SharedPools.Default<List<TerminalLexeme>>();
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = ObjectPoolExtensions.Allocate(pool);
            var expectedLexerRules = this.parseEngine.GetExpectedLexerRules();

            foreach (var rule in expectedLexerRules)
            {
                if (Equals(rule.LexerRuleType, TerminalLexerRule.TerminalLexerRuleType))
                {
                    expectedLexemes.Add(new TerminalLexeme(rule as TerminalLexerRule, Position));
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

            pool.ClearAndFree(expectedLexemes);

            if (firstPassingRule == null)
            {
                return false;
            }

            var result = this.parseEngine.Pulse(firstPassingRule);
            if (result)
            {
                this.capture.Append(c);
            }

            return result;
        }

        private readonly StringBuilder capture;
        private readonly IParseEngine parseEngine;
    }
}