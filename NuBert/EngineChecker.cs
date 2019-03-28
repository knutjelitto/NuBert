using Lingu.Builders;
using Lingu.Charts;
using Lingu.Grammars;

namespace NuBert
{
    public class EngineChecker
    {
        public void Check()
        {
            Check1();
        }


        private void Check1()
        {
            NonterminalExpr argument = "argument";
            NonterminalExpr argumentList = "argument-list";
            NonterminalExpr atom = "atom";

            TerminalExpr name = 'n';
            TerminalExpr number = '1';

            argument.Body = atom;
            argumentList.Body = argument | (argumentList + ',' + argument);
            atom.Body = name | number;

            var grammar = new GrammarBuilder().From(argumentList);

            var engine = new Engine(grammar);

            engine.Pulse(Token.Name);
            engine.Pulse(Token.Comma);
            engine.Pulse(Token.Number);
        }


        private class Token : IToken
        {
            private Token(string name)
            {
                this.name = name;
            }

            public bool IsFrom(Terminal terminal)
            {
                return terminal.Name == this.name;
            }

            public override string ToString()
            {
                return $"'{this.name}'";
            }

            private readonly string name;

            public static Token Name => new Token("n");
            public static Token Number => new Token("1");
            public static Token Comma => new Token(",");
        }

    }
}
