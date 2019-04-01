using System;
using Lingu.Automata;
using Lingu.Earley;
using Lingu.Grammars;
using Lingu.Grammars.Build;

namespace Lingu.Check
{
    public class EngineChecker
    {
        public void Check()
        {
            Check1();
        }


        private void Check1()
        {
            RuleExpr argument = "argument";
            RuleExpr argumentList = "argument-list";
            RuleExpr atom = "atom";

            var name = TerminalExpr.From(DfaProvision.From("name", 'n'));
            var number = TerminalExpr.From(DfaProvision.From("number", '1'));

            argument.Body = atom;
            argumentList.Body = argument | (argumentList + ',' + argument);
            atom.Body = name | number;

            var grammar = new GrammarBuilder().From(argumentList);

            var engine = new EarleyEngine(grammar);

            engine.Pulse(Token.Name);
            engine.Pulse(Token.Comma);
            engine.Pulse(Token.Number);
        }


        private class Token : IToken
        {
            private Token(string name, char value)
            {
                this.name = name;
                this.value = value;
            }

            public bool IsFrom(Terminal terminal)
            {
                return this.name == terminal.Name;
            }

            public override string ToString()
            {
                return $"'{this.value}'";
            }

            private readonly string name;
            private readonly char value;

            public static Token Name => new Token("name", 'a');
            public static Token Number => new Token("number", '1');
            public static Token Comma => new Token(",", ',');
        }

    }
}
