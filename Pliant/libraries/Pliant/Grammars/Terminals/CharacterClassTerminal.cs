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
            for (var i = 0; i < terminals.Count; i++)
            {
                var intervals = terminals[i].GetIntervals();
                intervalList.AddRange(intervals);
            }
            return Interval.Group(intervalList);
        }

        public override bool IsMatch(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            for (var t = 0; t < this._terminals.Count; t++)
            {
                var terminal = this._terminals[t];
                if (terminal.IsMatch(character))
                {
                    return true;
                }
            }
            return false;
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            if(this._intervals == null)
            {
                this._intervals = CreateIntervals(this._terminals);
            }

            return this._intervals;
        }
    }
}