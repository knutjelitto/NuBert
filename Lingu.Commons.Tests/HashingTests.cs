using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Commons.Tests
{
    [TestClass]
    public class HashingTests
    {
        [TestMethod]
        public void AddedHashShouldHashEqualForRandomOrder()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();

            var s1 = new[] { o1, o2, o3 };
            var s2 = new[] { o1, o3, o2 };
            var s3 = new[] { o2, o1, o3 };
            var s4 = new[] { o2, o3, o1 };
            var s5 = new[] { o3, o1, o2 };
            var s6 = new[] { o3, o2, o1 };

            Assert.AreEqual(s1.AddedHash(), s2.AddedHash());
            Assert.AreEqual(s2.AddedHash(), s3.AddedHash());
            Assert.AreEqual(s3.AddedHash(), s4.AddedHash());
            Assert.AreEqual(s4.AddedHash(), s5.AddedHash());
            Assert.AreEqual(s5.AddedHash(), s6.AddedHash());
        }

        [TestMethod]
        public void SequenceHashShouldHashDifferentForRandomOrder()
        {
            var o1 = 11;
            var o2 = 12;
            var o3 = 13;

            var s1 = new[] { o1, o2, o3 };
            var s2 = new[] { o1, o3, o2 };
            var s3 = new[] { o2, o1, o3 };
            var s4 = new[] { o2, o3, o1 };
            var s5 = new[] { o3, o1, o2 };
            var s6 = new[] { o3, o2, o1 };

            Assert.AreNotEqual(s1.SequenceHash(), s2.SequenceHash());
            Assert.AreNotEqual(s1.SequenceHash(), s3.SequenceHash());
            Assert.AreNotEqual(s1.SequenceHash(), s4.SequenceHash());
            Assert.AreNotEqual(s1.SequenceHash(), s5.SequenceHash());
            Assert.AreNotEqual(s1.SequenceHash(), s6.SequenceHash());

            Assert.AreNotEqual(s2.SequenceHash(), s3.SequenceHash());
            Assert.AreNotEqual(s2.SequenceHash(), s4.SequenceHash());
            Assert.AreNotEqual(s2.SequenceHash(), s5.SequenceHash());
            Assert.AreNotEqual(s2.SequenceHash(), s6.SequenceHash());

            Assert.AreNotEqual(s3.SequenceHash(), s4.SequenceHash());
            Assert.AreNotEqual(s3.SequenceHash(), s5.SequenceHash());
            Assert.AreNotEqual(s3.SequenceHash(), s6.SequenceHash());

            Assert.AreNotEqual(s4.SequenceHash(), s5.SequenceHash());
            Assert.AreNotEqual(s4.SequenceHash(), s6.SequenceHash());

            Assert.AreNotEqual(s5.SequenceHash(), s6.SequenceHash());
        }
    }
}
