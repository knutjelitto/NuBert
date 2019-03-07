using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal static class HashCode
    {
        private const uint seed = 2166136261;
        private const int incremental = 16777619;
        
        public static int Compute(int first, int second, int third, int fourth, int fifth, int sixth)
        {
            unchecked
            {
                var hash = (int)seed;
                hash = (hash * incremental) ^ first;
                hash = (hash * incremental) ^ second;
                hash = (hash * incremental) ^ third;
                hash = (hash * incremental) ^ fourth;
                hash = (hash * incremental) ^ fifth;
                hash = (hash * incremental) ^ sixth;
                return hash;
            }
        }

        public static int Compute(IEnumerable<object> items)
        {
            unchecked
            {
                var hash = (int)seed;
                foreach (var item in items)
                {
                    hash = (hash * incremental) ^ item.GetHashCode();
                }
                return hash;
            }
        }
        
        public static int ComputeIncrementalHash(int hashCode, int accumulator, bool isFirstValue = false)
        {
            unchecked
            {
                if (isFirstValue)
                {
                    accumulator = (int)seed;
                }
                accumulator = (accumulator * incremental) ^ hashCode;
                return accumulator;
            }
        }
    }
}