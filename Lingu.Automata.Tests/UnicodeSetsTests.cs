using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lingu.Automata.Tests
{
    [TestClass]
    public class UnicodeSetsTests
    {
        [TestMethod]
        public void AnyShouldHaveCardinality()
        {
            Assert.AreEqual(0x10000, UnicodeSets.Any.Cardinality);
        }

        [TestMethod]
        public void CategorySetsShouldHaveCardinality()
        {
            Assert.AreEqual(1064, UnicodeSets.Category[UnicodeCategory.UppercaseLetter].Cardinality);
            Assert.AreEqual(1369, UnicodeSets.Category[UnicodeCategory.LowercaseLetter].Cardinality);
            Assert.AreEqual(31, UnicodeSets.Category[UnicodeCategory.TitlecaseLetter].Cardinality);
            Assert.AreEqual(231, UnicodeSets.Category[UnicodeCategory.ModifierLetter].Cardinality);
            Assert.AreEqual(46023, UnicodeSets.Category[UnicodeCategory.OtherLetter].Cardinality);
            Assert.AreEqual(988, UnicodeSets.Category[UnicodeCategory.NonSpacingMark].Cardinality);
            Assert.AreEqual(259, UnicodeSets.Category[UnicodeCategory.SpacingCombiningMark].Cardinality);
            Assert.AreEqual(13, UnicodeSets.Category[UnicodeCategory.EnclosingMark].Cardinality);
            Assert.AreEqual(370, UnicodeSets.Category[UnicodeCategory.DecimalDigitNumber].Cardinality);
            Assert.AreEqual(65, UnicodeSets.Category[UnicodeCategory.LetterNumber].Cardinality);
            Assert.AreEqual(290, UnicodeSets.Category[UnicodeCategory.OtherNumber].Cardinality);
            Assert.AreEqual(17, UnicodeSets.Category[UnicodeCategory.SpaceSeparator].Cardinality);
            Assert.AreEqual(1, UnicodeSets.Category[UnicodeCategory.LineSeparator].Cardinality);
            Assert.AreEqual(1, UnicodeSets.Category[UnicodeCategory.ParagraphSeparator].Cardinality);
            Assert.AreEqual(65, UnicodeSets.Category[UnicodeCategory.Control].Cardinality);
            Assert.AreEqual(40, UnicodeSets.Category[UnicodeCategory.Format].Cardinality);
            Assert.AreEqual(2048, UnicodeSets.Category[UnicodeCategory.Surrogate].Cardinality);
            Assert.AreEqual(6400, UnicodeSets.Category[UnicodeCategory.PrivateUse].Cardinality);
            Assert.AreEqual(10, UnicodeSets.Category[UnicodeCategory.ConnectorPunctuation].Cardinality);
            Assert.AreEqual(24, UnicodeSets.Category[UnicodeCategory.DashPunctuation].Cardinality);
            Assert.AreEqual(75, UnicodeSets.Category[UnicodeCategory.OpenPunctuation].Cardinality);
            Assert.AreEqual(73, UnicodeSets.Category[UnicodeCategory.ClosePunctuation].Cardinality);
            Assert.AreEqual(12, UnicodeSets.Category[UnicodeCategory.InitialQuotePunctuation].Cardinality);
            Assert.AreEqual(10, UnicodeSets.Category[UnicodeCategory.FinalQuotePunctuation].Cardinality);
            Assert.AreEqual(390, UnicodeSets.Category[UnicodeCategory.OtherPunctuation].Cardinality);
            Assert.AreEqual(936, UnicodeSets.Category[UnicodeCategory.MathSymbol].Cardinality);
            Assert.AreEqual(53, UnicodeSets.Category[UnicodeCategory.CurrencySymbol].Cardinality);
            Assert.AreEqual(116, UnicodeSets.Category[UnicodeCategory.ModifierSymbol].Cardinality);
            Assert.AreEqual(2655, UnicodeSets.Category[UnicodeCategory.OtherSymbol].Cardinality);
            Assert.AreEqual(1907, UnicodeSets.Category[UnicodeCategory.OtherNotAssigned].Cardinality);
        }

        [TestMethod]
        public void UnkownUnicodeCategoryNameShouldBeNull()
        {
            Assert.IsNull(UnicodeSets.Category["Ab"]);
        }

        [TestMethod]
        public void StandardUnicodeCategoryNamesShouldBeTheSame()
        {
            Assert.AreEqual(UnicodeSets.Category[UnicodeCategory.LowercaseLetter], UnicodeSets.Category["Ll"]);
        }

        [TestMethod]
        public void StandardUnicodeCategoryNamesShoulbBeProperSubsets()
        {
            Assert.IsTrue(UnicodeSets.Category[UnicodeCategory.LowercaseLetter].IsProperSubsetOf(UnicodeSets.Category["L"]));
            Assert.IsTrue(UnicodeSets.Category[UnicodeCategory.UppercaseLetter].IsProperSubsetOf(UnicodeSets.Category["L"]));
            Assert.IsTrue(UnicodeSets.Category[UnicodeCategory.TitlecaseLetter].IsProperSubsetOf(UnicodeSets.Category["L"]));
            Assert.IsTrue(UnicodeSets.Category[UnicodeCategory.OtherLetter].IsProperSubsetOf(UnicodeSets.Category["L"]));
            Assert.IsTrue(UnicodeSets.Category[UnicodeCategory.ModifierLetter].IsProperSubsetOf(UnicodeSets.Category["L"]));
        }
    }
}