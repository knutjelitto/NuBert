using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class SetTerminal : Terminal
    {
        public SetTerminal(params char[] characters)
            : this(new HashSet<char>(characters))
        {
        }

        public SetTerminal(char first)
        {
            this._characterSet = new HashSet<char>();
            this._characterSet.Add(first);
        }

        public SetTerminal(char first, char second)
            : this(first)
        {
            this._characterSet.Add(second);
        }

        public SetTerminal(ISet<char> characterSet)
        {
            this._characterSet = new HashSet<char>(characterSet);
            this._intervals = CreateIntervals(this._characterSet);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            if (this._intervals == null)
            {
                this._intervals = CreateIntervals(this._characterSet);
            }

            return this._intervals;
        }

        public override bool IsMatch(char character)
        {
            return this._characterSet.Contains(character);
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, this._characterSet)}]";
        }

        private static IReadOnlyList<Interval> CreateIntervals(HashSet<char> characterSet)
        {
            var intervalList = new List<Interval>();

            // create a initial set of intervals
            foreach (var character in characterSet)
            {
                intervalList.Add(new Interval(character));
            }

            var groupedIntervals = Interval.Group(intervalList);

            return groupedIntervals;
        }

        private readonly HashSet<char> _characterSet;
        private IReadOnlyList<Interval> _intervals;
    }
}