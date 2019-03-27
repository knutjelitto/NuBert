using Lingu.Builders;
using Lingu.Charts;
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

            Assert.AreEqual(3, engine.Grammar.Productions.Count);
        }

        [TestMethod]
        public void ShouldParseNothing()
        {
            var engine = MakeEngine();

            Assert.IsTrue(engine.IsAccepted);
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
        public void ShouldntParseCACA()
        {
            var engine = MakeEngine();

            engine.Pulse(Token.Comma);
            engine.Pulse(Token.A);
            engine.Pulse(Token.Comma);
            engine.Pulse(Token.A);

            Assert.IsFalse(engine.IsAccepted);
        }

        private Engine MakeEngine()
        {
            NonterminalExpr list = "list";

            list.Body = 'a' | (list + ',' + 'a') | ChainExpr.Epsilon;

            var grammar = new GrammarBuilder().From(list);

            var engine = new Engine(grammar);

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