using System.Collections.Generic;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class SetTerminal : Terminal
    {
        public SetTerminal(params char[] characters)
            : this(characters.AsEnumerable())
        {
        }

        public SetTerminal(char first, char second)
        {
            this.characterSet = new HashSet<char> {first, second};
        }

        public SetTerminal(IEnumerable<char> characterSet)
        {
            this.characterSet = new HashSet<char>(characterSet);
            this.intervals = CreateIntervals(this.characterSet);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this.intervals ?? (this.intervals = CreateIntervals(this.characterSet));
        }

        public override bool IsMatch(char character)
        {
            return this.characterSet.Contains(character);
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, this.characterSet)}]";
        }

        private static IReadOnlyList<Interval> CreateIntervals(IEnumerable<char> characters)
        {
            var intervalListPool = SharedPools.Default<List<Interval>>();
            var intervalList = ObjectPoolExtensions.Allocate(intervalListPool);

            // create a initial set of intervals
            foreach (var character in characters)
            {
                intervalList.Add(new Interval(character));
            }

            var groupedIntervals = Interval.Group(intervalList);
            intervalListPool.ClearAndFree(intervalList);

            return groupedIntervals;
        }

        private readonly HashSet<char> characterSet;
        private IReadOnlyList<Interval> intervals;
    }
}