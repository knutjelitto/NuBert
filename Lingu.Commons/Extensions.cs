using System.Collections.Generic;
using System.Linq;

namespace Lingu.Commons
{
    public static class Extensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static IEnumerable<T> Plus<T>(this IEnumerable<T> items, T item)
        {
            return items.Concat(Enumerable.Repeat(item, 1));
        }
    }
}