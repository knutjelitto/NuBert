using System.Collections.Generic;
using System.Text;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Runtime
{
    public class ParseEngineLexeme : Lexeme
    {
        public ParseEngineLexeme(GrammarLexerRule lexer)
            : base(lexer, 0)
        {
            this.capture = new StringBuilder();
            this.parseEngine = new ParseEngine(lexer.Grammar);
        }

        public override string Value => this.capture.ToString();

        public override bool IsAccepted()
        {
            return this.parseEngine.IsAccepted();
        }

        public override bool Scan(char character)
        {
            var pool = SharedPools.Default<List<Lexeme>>();
            // get expected lexems
            // PERF: Avoid Linq where, let and select expressions due to lambda allocation
            var expectedLexemes = pool.Allocate();
            var expectedLexerRules = this.parseEngine.GetExpectedLexerRules();

            foreach (var rule in expectedLexerRules)
            {
                if (rule is TerminalLexer terminalRule)
                {
                    expectedLexemes.Add(terminalRule.CreateLexeme(Position));
                }
            }

            // filter on first rule to pass (since all rules are one character per lexeme)
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            Lexeme firstPassingRule = null;
            foreach (var lexeme in expectedLexemes)
            {
                if (lexeme.Scan(character))
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
                this.capture.Append(character);
            }

            return result;
        }

        private readonly StringBuilder capture;
        private readonly IParseEngine parseEngine;
    }
}