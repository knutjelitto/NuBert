using Lingu.Earley;
using Lingu.Grammars.Build;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Grammars.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void GrammarShouldBeCreatable()
        {
            var engine = MakeEngine();

            Assert.AreEqual(4, engine.Grammar.Productions.Count);
        }

        [TestMethod]
        public void ShouldntParseNothing()
        {
            var engine = MakeEngine();

            Assert.IsFalse(engine.IsAccepted);
        }

        [TestMethod]
        public void ShouldParseACA()
        {
            var engine = MakeEngine();

            engine.Pulse(Token.A);
            engine.Pulse(Token.Comma);
            engine.Pulse(Token.A);

            Assert.IsTrue(engine.IsAccepted);
        }

        [TestMethod]
        public void ShouldntParseACAC()
        {
            var engine = MakeEngine();

            engine.Pulse(Token.A);
            engine.Pulse(Token.Comma);
            engine.Pulse(Token.A);
            engine.Pulse(Token.Comma);

            Assert.IsFalse(engine.IsAccepted);
        }


        [TestMethod]
        public void ShouldntParseInitialC()
        {
            var engine = MakeEngine();

            var ok = engine.Pulse(Token.Comma);

            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void ShouldParseInitialA()
        {
            var engine = MakeEngine();

            var ok = engine.Pulse(Token.A);

            Assert.IsTrue(ok);
        }

        private EarleyEngine MakeEngine()
        {
            RuleExpr argument = "argument";
            RuleExpr argumentList = "argument-list";
            RuleExpr atom = "atom";

            argument.Body =
                atom;
            argumentList.Body = 
                argument | 
                (argumentList + ',' + argument);
            atom.Body = 
                'a';

            var grammar = new GrammarBuilder().From(argumentList);

            var engine = new EarleyEngine(grammar);

            return engine;
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

            private readonly string name;

            public static Token A => new Token("a");
            public static Token Comma => new Token(",");
        }
    }
}