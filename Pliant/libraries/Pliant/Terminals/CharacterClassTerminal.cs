using System.Collections.Generic;

namespace Pliant.Terminals
{
    public sealed class CharacterClassTerminal : Terminal
    {
        public CharacterClassTerminal(params Terminal[] terminals)
        {
            this.terminals = new List<Terminal>(terminals);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this.intervals ?? (this.intervals = CreateIntervals(this.terminals));
        }

        public override bool IsMatch(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            foreach (var terminal in this.terminals)
            {
                if (terminal.IsMatch(character))
                {
                    return true;
                }
            }

            return false;
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

        private IReadOnlyList<Interval> intervals;
        private readonly List<Terminal> terminals;
    }
}
