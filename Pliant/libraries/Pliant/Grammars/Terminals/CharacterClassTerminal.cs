using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class CharacterClassTerminal : Terminal
    {
        private readonly List<Terminal> _terminals;
        private IReadOnlyList<Interval> _intervals;

        public CharacterClassTerminal(params Terminal[] terminals)
        {
            this._terminals = new List<Terminal>(terminals);
        }

        private static IReadOnlyList<Interval> CreateIntervals(IReadOnlyList<Terminal> terminals)
        {
            var intervalList = new List<Interval>();
            foreach (var terminal in terminals)
            {
                var intervals = terminal.GetIntervals();
                intervalList.AddRange(intervals);
            }
            return Interval.Group(intervalList);
        }

        public override bool IsMatch(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            foreach (var terminal in this._terminals)
            {
                if (terminal.IsMatch(character))
                {
                    return true;
                }
            }

            return false;
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this._intervals ?? (this._intervals = CreateIntervals(this._terminals));
        }
    }
}