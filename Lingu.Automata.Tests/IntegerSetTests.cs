using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class IntegerSetTests
    {
        [TestMethod]
        public void TestSimple()
        {
            var sut = new IntegerSet((10, 20));

            Assert.AreEqual("[10-20]", sut.ToString());
        }

        [TestMethod]
        public void AddAfter()
        {
            var sut = new IntegerSet((10, 20), (21, 30));

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void AddAfterGap()
        {
            var sut = new IntegerSet((10, 19), (21, 30));

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void AddAfterOverlap()
        {
            var sut = new IntegerSet((10, 21), (21, 30));

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void AddBefore()
        {
            var sut = new IntegerSet((21, 30), (10, 20));

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void AddBeforeGap()
        {
            var sut = new IntegerSet((21, 30), (10, 19));

            Assert.AreEqual("[10-19,21-30]", sut.ToString());
        }

        [TestMethod]
        public void AddLarge()
        {
            var sut = new IntegerSet((21, 30), (10, 19), (1, 1000));

            Assert.AreEqual("[1-1000]", sut.ToString());
        }

        [TestMethod]
        public void AddBeforeOverlap()
        {
            var sut = new IntegerSet((21, 30), (10, 21));

            Assert.AreEqual("[10-30]", sut.ToString());
        }

        [TestMethod]
        public void AddFillGap()
        {
            var sut = new IntegerSet((1, 9), (11, 20));

            Assert.AreEqual("[1-9,11-20]", sut.ToString());

            sut.Add(10);

            Assert.AreEqual("[1-20]", sut.ToString());
        }

        [TestMethod]
        public void SubFromFront()
        {
            var sut = new IntegerSet((101, 200));

            sut.Sub((91, 110));

            Assert.AreEqual("[111-200]", sut.ToString());
        }

        [TestMethod]
        public void SubFromEnd()
        {
            var sut = new IntegerSet((101, 200));

            sut.Sub((191, 210));

            Assert.AreEqual("[101-190]", sut.ToString());
        }

        [TestMethod]
        public void SubChecker()
        {
            var sut = new IntegerSet((101, 200));

            sut.Sub((111, 190));

            Assert.AreEqual("[101-110,191-200]", sut.ToString());
        }

        [TestMethod]
        public void SubChecker2()
        {
            var sut = new IntegerSet((101, 200), (301, 400));

            sut.Sub((111, 390));

            Assert.AreEqual("[101-110,391-400]", sut.ToString());
        }

        [TestMethod]
        public void SubChecker3()
        {
            var sut = new IntegerSet((101, 200), (301, 400));

            sut.Sub((201, 390));

            Assert.AreEqual("[101-200,391-400]", sut.ToString());
        }

        [TestMethod]
        public void SubChecker4()
        {
            var sut = new IntegerSet((101, 200), (301, 400));

            sut.Sub((200, 301));

            Assert.AreEqual("[101-199,302-400]", sut.ToString());
        }

        [TestMethod]
        public void SubChecker5()
        {
            var sut = new IntegerSet((1, 10), (21, 30));

            for (var i = 0; i < 40; ++i)
            {
                sut.Sub(i);
            }

            Assert.AreEqual("[]", sut.ToString());
        }

        [TestMethod]
        public void SubLarge()
        {
            var sut = new IntegerSet((101, 200), (301, 400), (901, 1000));

            sut.Sub((1, 1000));

            Assert.AreEqual("[]", sut.ToString());
        }


        [TestMethod]
        public void Parsing()
        {
            Assert.IsTrue(IntegerSet.TryParse("[1-10,12-33]", out var set));

            Assert.AreEqual(new IntegerSet((1, 10), (12, 33)), set);
        }

        [TestMethod]
        public void SupersetOf()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (21, 30), (41, 50));

            Assert.IsTrue(set1.IsSupersetOf(set1));
            Assert.IsTrue(set2.IsSupersetOf(set2));

            Assert.IsFalse(set1.IsSupersetOf(set2));
            Assert.IsTrue(set2.IsSupersetOf(set1));
        }

        [TestMethod]
        public void SubsetOf()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (21, 30), (41, 50));

            Assert.IsTrue(set1.IsSubsetOf(set1));
            Assert.IsTrue(set2.IsSubsetOf(set2));

            Assert.IsTrue(set1.IsSubsetOf(set2));
            Assert.IsFalse(set2.IsSubsetOf(set1));
        }

        [TestMethod]
        public void ProperSupersetOf()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (21, 30), (41, 50));

            Assert.IsFalse(set1.IsProperSupersetOf(set1));
            Assert.IsFalse(set2.IsProperSupersetOf(set2));

            Assert.IsFalse(set1.IsProperSupersetOf(set2));
            Assert.IsTrue(set2.IsProperSupersetOf(set1));
        }

        [TestMethod]
        public void ProperSubsetOf()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (21, 30), (41, 50));

            Assert.IsFalse(set1.IsProperSubsetOf(set1));
            Assert.IsFalse(set2.IsProperSubsetOf(set2));

            Assert.IsTrue(set1.IsProperSubsetOf(set2));
            Assert.IsFalse(set2.IsProperSubsetOf(set1));
        }

        [TestMethod]
        public void UnionWith()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (41, 50));

            var union = set1.UnionWith(set2);

            Assert.AreEqual("[1-10,21-30]", set1.ToString());
            Assert.AreEqual("[1-10,41-50]", set2.ToString());
            Assert.AreEqual("[1-10,21-30,41-50]", union.ToString());
        }


        [TestMethod]
        public void ExeptWith()
        {
            var set1 = new IntegerSet((1, 10), (21, 30));
            var set2 = new IntegerSet((1, 10), (41, 50));

            var set12 = set1.ExceptWith(set2);
            var set21 = set2.ExceptWith(set1);

            Assert.AreEqual("[1-10,21-30]", set1.ToString());
            Assert.AreEqual("[1-10,41-50]", set2.ToString());
            Assert.AreEqual("[21-30]", set12.ToString());
            Assert.AreEqual("[41-50]", set21.ToString());
        }

        [TestMethod]
        public void Overlaps()
        {
            var set = new IntegerSet((10, 11), (14, 15), (18, 19));

            Assert.IsFalse(new IntegerSet((1, 2)).Overlaps(set));
            Assert.IsFalse(new IntegerSet((8, 9)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((9, 10)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((10, 11)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((11, 12)).Overlaps(set));
            Assert.IsFalse(new IntegerSet((12, 13)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((13, 14)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((14, 15)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((15, 16)).Overlaps(set));
            Assert.IsFalse(new IntegerSet((16, 17)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((17, 18)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((18, 19)).Overlaps(set));
            Assert.IsTrue(new IntegerSet((19, 20)).Overlaps(set));
            Assert.IsFalse(new IntegerSet((20, 21)).Overlaps(set));

            Assert.IsFalse(set.Overlaps(new IntegerSet((8, 9), (12, 13), (16, 17))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 10), (12, 13), (16, 17))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 9), (11, 13), (16, 17))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 9), (12, 14), (16, 17))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 9), (12, 14), (15, 17))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 9), (12, 14), (16, 18))));
            Assert.IsFalse(set.Overlaps(new IntegerSet((8, 9), (12, 13), (16, 17), (20, 21))));
            Assert.IsTrue(set.Overlaps(new IntegerSet((8, 9), (12, 13), (16, 17), (19, 21))));
        }
    }
}