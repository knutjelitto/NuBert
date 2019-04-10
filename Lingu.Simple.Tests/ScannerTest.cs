using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Simple.Tests
{
    [TestClass]
    public class ScannerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ScannerException))]
        public void IncompleteSingleCommentShouldThrow()
        {
            Make("  /").Next();
        }

        [TestMethod]
        public void IdentifierShouldSucceed()
        {
            Assert.IsTrue(First("a").IsFrom(Scanner.Identifier));
            Assert.IsTrue(First("aa-b12").IsFrom(Scanner.Identifier));
            Assert.IsTrue(First("aa1---cabc").IsFrom(Scanner.Identifier));
        }

        [TestMethod]
        [ExpectedException(typeof(ScannerException))]
        public void IdentifierErrorShouldThrow()
        {
            Assert.IsTrue(First("a-").IsFrom(Scanner.Identifier));
        }

        private Scanner Make(string text)
        {
            return new Scanner(new Input(text));
        }

        private SToken First(string text)
        {
            return new Scanner(new Input(text)).Next();
        }
    }
}
