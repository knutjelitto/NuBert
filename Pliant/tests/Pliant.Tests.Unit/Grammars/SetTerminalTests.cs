﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Terminals;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class SetTerminalTests
    {
        [TestMethod]
        public void SetTerminalShouldRecognizeCharacterInSet()
        {
            var setTerminal = new SetTerminal('a', 'z');
            Assert.IsTrue(setTerminal.IsMatch('a'));
            Assert.IsTrue(setTerminal.IsMatch('z'));
        }

        [TestMethod]
        public void SetTerminalShouldNotRecognizeCharacterOutOfSet()
        {
            var setTerminal = new SetTerminal('a', 'z');
            Assert.IsFalse(setTerminal.IsMatch('b'));
        }


        [TestMethod]
        public void SetTerminalGetIntervalsShouldReturnMergedIntervalWhenCharactersOverlap()
        {
            var setTerminal = new SetTerminal('a', 'b', 'c', 'd', 'e', 'z');
            var intervals = setTerminal.GetIntervals();
            Assert.AreEqual(2, intervals.Count);
            Assert.AreEqual('a', intervals[0].Min);
            Assert.AreEqual('e', intervals[0].Max);
            Assert.AreEqual('z', intervals[1].Min);
            Assert.AreEqual('z', intervals[1].Max);
        }

        [TestMethod]
        public void SetTerminalShouldUseIntervalsForToString()
        {
            var setTerminal = new SetTerminal('a', 'b', 'c', 'd', 'e', 'z');

            Assert.AreEqual("[a-ez]", setTerminal.ToString());

            setTerminal = new SetTerminal('z', 'a', 'b', 'c', 'd', 'e');

            Assert.AreEqual("[a-ez]", setTerminal.ToString());

            setTerminal = new SetTerminal('a', 'b', 'c', 'd', 'e');

            Assert.AreEqual("[a-e]", setTerminal.ToString());

            setTerminal = new SetTerminal('z');

            Assert.AreEqual("[z]", setTerminal.ToString());
        }
    }
}
