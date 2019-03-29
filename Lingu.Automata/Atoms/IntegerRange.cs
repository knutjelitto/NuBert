using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lingu.Automata
{
    public struct IntegerRange : IEnumerable<int>
    {
        public IntegerRange(int minmax)
        {
            Min = minmax;
            Max = minmax;
        }

        public IntegerRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Max { get; }
        public int Min { get; }

        public bool Contains(int value)
        {
            return Min <= value && value <= Max;
        }

        public bool Overlaps(IntegerRange other)
        {
            return Contains(other.Min) || Contains(other.Max) || other.Contains(Min) || other.Contains(Max);
        }

        public int Count => Max - Min + 1;

        public IEnumerator<int> GetEnumerator() => Enumerable.Range(Min, Max - Min + 1).GetEnumerator();

        public override string ToString()
        {
            if (Min == Max)
            {
                return $"{Min}";
            }

            return $"{Min}-{Max}";
        }

        public static bool TryParse(string str, out IntegerRange range)
        {
            var end = 0;
            while (end < str.Length && char.IsDigit(str, end))
            {
                end += 1;
            }
            if (end > 0)
            {
                if (end == str.Length)
                {
                    if (int.TryParse(str, out var minmax))
                    {
                        range = new IntegerRange(minmax);
                        return true;
                    }
                }
                else if (str[end] == '-' && int.TryParse(str.Substring(0, end), out var min))
                {
                    var start = end = end + 1;
                    while (end < str.Length && char.IsDigit(str, end))
                    {
                        end += 1;
                    }
                    if (end > start && end == str.Length)
                    {
                        if (int.TryParse(str.Substring(start, end-start), out var max))
                        {
                            range = new IntegerRange(min, max);
                            return true;
                        }
                    }
                }
            }

            range = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is IntegerRange other && Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode() => (Min, Max).GetHashCode();
    }
}