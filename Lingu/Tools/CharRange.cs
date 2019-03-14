using System;
using System.Diagnostics;

namespace Lingu.Tools
{
    public struct CharRange : IComparable<CharRange>
    {
        public CharRange(char min, char max)
        {
            Debug.Assert(min <= max);
            Min = min;
            Max = max;
        }

        public CharRange(ushort min, ushort max)
            : this((char)min, (char)max)
        {
        }

        public ushort Max { get; }
        public ushort Min { get; }

        public bool Overlaps(CharRange other)
        {
            return Contains(other.Min) || Contains(other.Max);
        }

        public bool Contains(ushort value)
        {
            return Min <= value && value <= Max;
        }

        public int CompareTo(CharRange other)
        {
            var maxComparison = Max.CompareTo(other.Max);
            return maxComparison != 0 ? maxComparison : Min.CompareTo(other.Min);
        }

        public override string ToString()
        {
            if (Min == Max)
            {
                return $"{(int) Min}";
            }

            return $"{(int) Min}-{(int) Max}";
        }
    }
}