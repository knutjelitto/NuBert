using System.Collections.Generic;
using System.Text;
using Pliant.Charts;

namespace Pliant.Utilities
{
    public static class ObjectPoolExtensions
    {
        internal static Queue<T> Allocate<T>(this ObjectPool<Queue<T>> pool)
        {
            var queue = pool.Allocate();
            //queue.Clear();
            return queue;
        }

        internal static HashSet<TValue> Allocate<TValue>(this ObjectPool<HashSet<TValue>> pool)
        {
            var set = pool.Allocate();
            //set.Clear();
            return set;
        }

        internal static SortedSet<TValue> Allocate<TValue>(this ObjectPool<SortedSet<TValue>> pool)
        {
            var set = pool.Allocate();
            //set.Clear();
            return set;
        }

        internal static HashSet<DottedRule> Allocate(this ObjectPool<HashSet<DottedRule>> pool)
        {
            var set = pool.Allocate();
            //set.Clear();
            return set;
        }

        internal static Dictionary<TKey, TValue> Allocate<TKey, TValue>(this ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            var dictionary = pool.Allocate();
            //dictionary.Clear();
            return dictionary;
        }

        internal static List<T> Allocate<T>(this ObjectPool<List<T>> pool)
        {
            var list = pool.Allocate();
            //list.Clear();
            return list;
        }

        internal static StringBuilder Allocate(this ObjectPool<StringBuilder> pool)
        {
            var builder = pool.Allocate();
            //builder.Clear();
            return builder;
        }

        internal static void ClearAndFree<T>(this ObjectPool<Queue<T>> pool, Queue<T> queue)
        {
            queue.Clear();
            pool.Free(queue);
        }

        internal static void ClearAndFree<TValue>(this ObjectPool<HashSet<TValue>> pool, HashSet<TValue> set)
        {
            set.Clear();
            pool.Free(set);
        }

        internal static void ClearAndFree<TValue>(this ObjectPool<SortedSet<TValue>> pool, SortedSet<TValue> set)
        {
            set.Clear();
            pool.Free(set);
        }

        internal static void ClearAndFree(this ObjectPool<HashSet<DottedRule>> pool, HashSet<DottedRule> set)
        {
            set.Clear();
            pool.Free(set);
        }

        internal static void ClearAndFree<TKey, TValue>(this ObjectPool<Dictionary<TKey, TValue>> pool, Dictionary<TKey, TValue> dictionary)
        {
            dictionary.Clear();
            pool.Free(dictionary);
        }

        internal static void ClearAndFree<T>(this ObjectPool<List<T>> pool, List<T> list)
        {
            if (list == null)
            {
                return;
            }

            if (pool == null)
            {
                return;
            }

            list.Clear();
            pool.Free(list);
        }

        internal static void ClearAndFree(this ObjectPool<StringBuilder> pool, StringBuilder builder)
        {
            if (pool == null)
            {
                return;
            }

            if (builder == null)
            {
                return;
            }

            builder.Clear();
            pool.Free(builder);
        }
    }
}