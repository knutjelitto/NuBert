using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal static class HashCode
    {
        private const uint Seed = 2166136261;
        private const int Incremental = 16777619;
        
        public static int Compute(int first)
        {
            unchecked
            {
                var hash = (int)Seed;
                hash = hash * Incremental ^ first;
                return hash;
            }
        }

        public static int Compute(int first, int second)
        {
            unchecked
            {
                var hash = (int)Seed;
                hash = hash * Incremental ^ first;
                hash = hash * Incremental ^ second;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third)
        {
            unchecked
            {
                var hash = (int)Seed;
                hash = hash * Incremental ^ first;
                hash = hash * Incremental ^ second;
                hash = hash * Incremental ^ third;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third, int fourth)
        {
            unchecked
            {
                var hash = (int)Seed;
                hash = hash * Incremental ^ first;
                hash = hash * Incremental ^ second;
                hash = hash * Incremental ^ third;
                hash = hash * Incremental ^ fourth;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third, int fourth, int fifth, int sixth)
        {
            unchecked
            {
                var hash = (int)Seed;
                hash = hash * Incremental ^ first;
                hash = hash * Incremental ^ second;
                hash = hash * Incremental ^ third;
                hash = hash * Incremental ^ fourth;
                hash = hash * Incremental ^ fifth;
                hash = hash * Incremental ^ sixth;
                return hash;
            }
        }

        public static int Compute(IEnumerable<object> items)
        {
            unchecked
            {
                var hash = (int)Seed;
                foreach (var item in items)
                {
                    hash = hash * Incremental ^ item.GetHashCode();
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
                    accumulator = (int)Seed;
                }
                accumulator = accumulator * Incremental ^ hashCode;
                return accumulator;
            }
        }
    }
}