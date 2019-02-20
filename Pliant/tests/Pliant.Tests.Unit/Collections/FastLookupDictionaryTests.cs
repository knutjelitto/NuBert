using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit.Collections
{
    [TestClass]
    public class FastLookupDictionaryTests
    {
        private sealed class Element
        {
            private int Value { get; }

            public Element(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                return obj is Element other && Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        [TestMethod]
        public void FastLookupDictionaryTryGetValueShouldReturnValueWithSameHashCode()
        {
            // create two elements with the same hashcode
            var first = new Element(1);
            var second = new Element(1);

            var fastLookupDictionary = new Dictionary<Element, Element> {[first] = second};

            Assert.IsTrue(fastLookupDictionary.TryGetValue(second, out var third));
            Assert.IsTrue(ReferenceEquals(third, second));
        }

        [TestMethod]
        public void FastLookupDictionaryTryGetValueShouldContainAllValuesOfLargeList()
        {
            var fastLookupDictionary = new Dictionary<int, int>();
            for (var i = 0; i < 50; i++)
            {
                fastLookupDictionary.Add(i, i);
                Assert.IsTrue(fastLookupDictionary.TryGetValue(i, out var _));
            }
        }


        [TestMethod]
        public void FastLookupDictionaryGetValueShouldContainAllValuesOfLargeList()
        {
            var fastLookupDictionary = new Dictionary<int, int>();
            for (var i = 0; i < 50; i++)
            {
                fastLookupDictionary.Add(i, i);
                var value = fastLookupDictionary[i];
                Assert.AreEqual(i, value);
            }
        }

        [TestMethod]
        public void FastLookupDictionaryGetValueShouldContainAllValuesOfSmallList()
        {
            var fastLookupDictionary = new Dictionary<int, int>();
            for (var i = 0; i < 2; i++)
            {
                fastLookupDictionary.Add(i, i);
                var value = fastLookupDictionary[i];
                Assert.AreEqual(i, value);
            }
        }
    }
}
