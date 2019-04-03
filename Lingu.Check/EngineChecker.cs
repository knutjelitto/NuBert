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

            engine.Pulse(XToken.Name);
            engine.Pulse(XToken.Comma);
            engine.Pulse(XToken.Number);
        }


        private class XToken : IToken
        {
            private XToken(string name, char value)
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

            public static XToken Name => new XToken("name", 'a');
            public static XToken Number => new XToken("number", '1');
            public static XToken Comma => new XToken(",", ',');
        }

    }
}
