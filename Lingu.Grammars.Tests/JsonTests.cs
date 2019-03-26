using Lingu.Samples.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Grammars.Tests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void CreateGrammar()
        {
            var grammar = JsonGrammar.Create();

            Assert.AreEqual(17, grammar.Productions.Count);
            Assert.AreEqual(11, grammar.Terminals.Count);
            Assert.AreEqual(7, grammar.Nonterminals.Count);
        }
    }
}
