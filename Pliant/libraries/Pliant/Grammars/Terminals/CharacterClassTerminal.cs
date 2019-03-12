using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Terminals
{
    public sealed class CharacterClassTerminal : AtomTerminal
    {
        public CharacterClassTerminal(params AtomTerminal[] terminals)
        {
            this.terminals = new List<AtomTerminal>(terminals);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this.intervals ?? (this.intervals = CreateIntervals(this.terminals));
        }

        public override bool CanApply(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            foreach (var terminal in this.terminals)
            {
                if (terminal.CanApply(character))
                {
                    return true;
                }
            }

            return false;
        }

        private static IReadOnlyList<Interval> CreateIntervals(IReadOnlyList<AtomTerminal> terminals)
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
        private readonly List<AtomTerminal> terminals;
    }
}
