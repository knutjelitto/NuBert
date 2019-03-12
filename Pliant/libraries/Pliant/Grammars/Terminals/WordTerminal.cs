using System.Collections.Generic;

namespace Pliant.Terminals
{
    public class WordTerminal : AtomTerminal
    {
        private WordTerminal()
        {
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return intervals;
        }

        public override bool CanApply(char character)
        {
            return 'a' <= character && character <= 'z' ||
                   'A' <= character && character <= 'Z' ||
                   '0' <= character && character <= '9' ||
                   '_' == character;
        }

        public static readonly WordTerminal Instance = new WordTerminal();

        private static readonly Interval[] intervals =
        {
            new Interval('a', 'z'),
            new Interval('A', 'Z'),
            new Interval('0', '9'),
            new Interval('_')
        };
    }
}