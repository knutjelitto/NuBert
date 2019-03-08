using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal static class HashCode
    {
        private const uint seed = 2166136261;
        private const int incremental = 16777619;

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

        public static int Compute(IEnumerable<int> items)
        {
            unchecked
            {
                var hash = (int)seed;
                foreach (var item in items)
                {
                    hash = (hash * incremental) ^ item;
                }
                return hash;
            }
        }
    }
}