using System.Globalization;
using Lingu.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Lingu.Tools
{
    [TestClass]
    public class IntegerSetTests
    {
        [TestMethod]
        public void TestSimple()
        {
            var sut = new IntegerSet {new IntegerRange(10, 20)};

            Assert.AreEqual("[10-20]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfter()
        {
            var sut = new IntegerSet { new IntegerRange(10, 20), new IntegerRange(21, 30) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfterGap()
        {
            var sut = new IntegerSet { new IntegerRange(10, 19), new IntegerRange(21, 30) };

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddAfterOverlap()
        {
            var sut = new IntegerSet { new IntegerRange(10, 21), new IntegerRange(21, 30) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBefore()
        {
            var sut = new IntegerSet { new IntegerRange(21, 30), new IntegerRange(10, 20) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBeforeGap()
        {
            var sut = new IntegerSet { new IntegerRange(21, 30), new IntegerRange(10, 19) };

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void AddLarge()
        {
            var sut = new IntegerSet { new IntegerRange(21, 30), new IntegerRange(10, 19), new IntegerRange(1, 1000) };

            Assert.AreEqual("[1-1000]", sut.ToString());
        }

        [TestMethod]
        public void TestAddBeforeOverlap()
        {
            var sut = new IntegerSet { new IntegerRange(21, 30), new IntegerRange(10, 21) };

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void SubFromFront()
        {
            var sut = new IntegerSet { new IntegerRange(101, 200) };

            sut.Sub(new IntegerRange(91, 110));

            Assert.AreEqual("[111-200]", sut.ToString());
        }

        [TestMethod]
        public void SubFromEnd()
        {
            var sut = new IntegerSet { new IntegerRange(101, 200) };

            sut.Sub(new IntegerRange(191, 210));

            Assert.AreEqual("[101-190]", sut.ToString());
        }

        [TestMethod]
        public void SubInner()
        {
            var sut = new IntegerSet { new IntegerRange(101, 200) };

            sut.Sub(new IntegerRange(111, 190));

            Assert.AreEqual("[101-110,191-200]", sut.ToString());
        }

        [TestMethod]
        public void SubLarge()
        {
            var sut = new IntegerSet { new IntegerRange(101, 200), new IntegerRange(301, 400), new IntegerRange(901, 1000) };

            sut.Sub(new IntegerRange(1, 1000));

            Assert.AreEqual("[]", sut.ToString());
        }


        [TestMethod]
        public void Parsing()
        {
            Assert.IsTrue(IntegerSet.TryParse("[1-10,12-33]", out var set));

            Assert.AreEqual(new IntegerSet { new IntegerRange(1, 10), new IntegerRange(12, 33)}, set);
        }

        [TestMethod]
        public void SupersetOf()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30), new IntegerRange(41, 50) };

            Assert.IsTrue(set1.IsSupersetOf(set1));
            Assert.IsTrue(set2.IsSupersetOf(set2));

            Assert.IsFalse(set1.IsSupersetOf(set2));
            Assert.IsTrue(set2.IsSupersetOf(set1));
        }

        [TestMethod]
        public void SubsetOf()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30), new IntegerRange(41, 50) };

            Assert.IsTrue(set1.IsSubsetOf(set1));
            Assert.IsTrue(set2.IsSubsetOf(set2));

            Assert.IsTrue(set1.IsSubsetOf(set2));
            Assert.IsFalse(set2.IsSubsetOf(set1));
        }

        [TestMethod]
        public void ProperSupersetOf()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30), new IntegerRange(41, 50) };

            Assert.IsFalse(set1.IsProperSupersetOf(set1));
            Assert.IsFalse(set2.IsProperSupersetOf(set2));

            Assert.IsFalse(set1.IsProperSupersetOf(set2));
            Assert.IsTrue(set2.IsProperSupersetOf(set1));
        }

        [TestMethod]
        public void ProperSubsetOf()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30), new IntegerRange(41, 50) };

            Assert.IsFalse(set1.IsProperSubsetOf(set1));
            Assert.IsFalse(set2.IsProperSubsetOf(set2));

            Assert.IsTrue(set1.IsProperSubsetOf(set2));
            Assert.IsFalse(set2.IsProperSubsetOf(set1));
        }

        [TestMethod]
        public void UnionWith()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(41, 50) };

            var set = set1.UnionWith(set2);

            Assert.AreEqual("[1-10,21-30]", set1.ToString());
            Assert.AreEqual("[1-10,41-50]", set2.ToString());
            Assert.AreEqual("[1-10,21-30,41-50]", set.ToString());
        }


        [TestMethod]
        public void ExeptWith()
        {
            var set1 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(21, 30) };
            var set2 = new IntegerSet { new IntegerRange(1, 10), new IntegerRange(41, 50) };

            var set12 = set1.ExceptWith(set2);
            var set21 = set2.ExceptWith(set1);

            Assert.AreEqual("[1-10,21-30]", set1.ToString());
            Assert.AreEqual("[1-10,41-50]", set2.ToString());
            Assert.AreEqual("[21-30]", set12.ToString());
            Assert.AreEqual("[41-50]", set21.ToString());
        }
    }
}