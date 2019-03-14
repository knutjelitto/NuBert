using Lingu.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Tools
{
    [TestClass]
    public class CharSetTests
    {
        [TestMethod]
        public void TestSimple()
        {
            var sut = new CharSet {new CharRange(10, 20)};

            Assert.AreEqual("[10-20]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfter()
        {
            var sut = new CharSet { new CharRange(10, 20), new CharRange(21, 30) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfterGap()
        {
            var sut = new CharSet { new CharRange(10, 19), new CharRange(21, 30) };

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfterOverlap()
        {
            var sut = new CharSet { new CharRange(10, 21), new CharRange(21, 30) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBefore()
        {
            var sut = new CharSet { new CharRange(21, 30), new CharRange(10, 20) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBeforeGap()
        {
            var sut = new CharSet { new CharRange(21, 30), new CharRange(10, 19) };

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBeforeOverlap()
        {
            var sut = new CharSet { new CharRange(21, 30), new CharRange(10, 21) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }
    }
}