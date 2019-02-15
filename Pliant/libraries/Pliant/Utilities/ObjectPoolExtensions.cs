﻿using System.Collections.Generic;
using System.Text;
using Pliant.Collections;

namespace Pliant.Utilities
{
    public static class ObjectPoolExtensions
    {
        internal static Queue<T> AllocateAndClear<T>(this ObjectPool<Queue<T>> pool)
        {
            var queue = pool.Allocate();
            queue.Clear();
            return queue;
        }

        internal static UniqueList<T> AllocateAndClear<T>(this ObjectPool<UniqueList<T>> pool)
        {
            var list = pool.Allocate();
            list.Clear();
            return list;
        }

        internal static FastLookupDictionary<TKey, TValue> AllocateAndClear<TKey, TValue>(
            this ObjectPool<FastLookupDictionary<TKey, TValue>> pool)
        {
            var dictionary = pool.Allocate();
            dictionary.Clear();
            return dictionary;
        }

        internal static HashSet<TValue> AllocateAndClear<TValue>(this ObjectPool<HashSet<TValue>> pool)
        {
            var hashSet = pool.Allocate();
            hashSet.Clear();
            return hashSet;
        }

        internal static SortedSet<TValue> AllocateAndClear<TValue>(this ObjectPool<SortedSet<TValue>> pool)
        {
            var hashSet = pool.Allocate();
            hashSet.Clear();
            return hashSet;
        }

        internal static Dictionary<TKey, TValue> AllocateAndClear<TKey, TValue>(this ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            var dictionary = pool.Allocate();
            dictionary.Clear();
            return dictionary;
        }

        internal static List<T> AllocateAndClear<T>(this ObjectPool<List<T>> pool)
        {
            var list = pool.Allocate();
            list.Clear();
            return list;
        }

        internal static StringBuilder AllocateAndClear(this ObjectPool<StringBuilder> pool)
        {
            var builder = pool.Allocate();
            builder.Clear();
            return builder;
        }

        internal static void ClearAndFree<T>(this ObjectPool<Queue<T>> pool, Queue<T> queue)
        {
            queue.Clear();
            pool.Free(queue);
        }

        internal static void ClearAndFree<T>(this ObjectPool<UniqueList<T>> pool, UniqueList<T> list)
        {
            list.Clear();
            pool.Free(list);
        }

        internal static void ClearAndFree<TValue>(this ObjectPool<HashSet<TValue>> pool, HashSet<TValue> hashSet)
        {
            hashSet.Clear();
            pool.Free(hashSet);
        }

        internal static void ClearAndFree<TValue>(this ObjectPool<SortedSet<TValue>> pool, SortedSet<TValue> hashSet)
        {
            hashSet.Clear();
            pool.Free(hashSet);
        }

        internal static void ClearAndFree<TKey, TValue>(this ObjectPool<Dictionary<TKey, TValue>> pool,
                                                        Dictionary<TKey, TValue> dictionary)
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