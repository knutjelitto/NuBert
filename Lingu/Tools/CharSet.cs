using System;
using System.Collections;
using System.Collections.Generic;

namespace Lingu.Tools
{
    public class CharSet : IReadOnlyCollection<CharRange>
    {
        public CharSet()
        {
            this.ranges = new List<CharRange>();
        }

        public int Count => this.ranges.Count;

        public void Add(CharRange add)
        {
            var i = 0;
            while (i < this.ranges.Count)
            {
                var current = this.ranges[i];

                if (add.Min > current.Max + 1)
                {
                    ++i;
                    continue;
                }

                if (add.Max + 1 < current.Min)
                {
                    // before current
                    this.ranges.Insert(i, add);
                    return;
                }

                if (add.Max <= current.Max)
                {
                    // combine with current
                    this.ranges[i] = new CharRange((char) Math.Min(add.Min, current.Min), current.Max);
                    return;
                }

                add = new CharRange((char) Math.Min(add.Min, current.Min), add.Max);
                this.ranges.RemoveAt(i);
            }

            if (i == this.ranges.Count)
            {
                this.ranges.Add(add);
            }
        }

        public void Sub(CharRange sub)
        {
            var i = 0;
            while (i < this.ranges.Count)
            {
                var current = this.ranges[i];

                if (sub.Max >= current.Min)
                {
                    if (sub.Max <= current.Max)
                    {

                    }
                }
                if (sub.Max <= current.Max)
                {

                }
                if (sub.Overlaps(current))
                {

                }
                else
                {
                    i++;
                    continue;
                }
            }
        }

        public IEnumerator<CharRange> GetEnumerator()
        {
            return this.ranges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"[{string.Join(",", this.ranges)}]";
        }

        private readonly List<CharRange> ranges;
    }
}