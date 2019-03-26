using System.Collections.Generic;

namespace Lingu.Commons
{
    public static class Hashing
    {
        public static int SequenceHash<T>(this IEnumerable<T> values)
        {
            const int b = 378551;
            var a = 63689;
            var hash = 0;

            // If it overflows then just wrap around
            unchecked
            {
                foreach (var value in values)
                {
                    hash = hash * a + value.GetHashCode();
                    a = a * b;

                }
            }

            return hash;
        }

        public static int AddedHash<T>(this IEnumerable<T> values)
        {
            var hash = 0;

            // If it overflows then just wrap around
            unchecked
            {
                foreach (var value in values)
                {
                    hash = hash + value.GetHashCode();
                }
            }

            return hash;
        }
    }
}