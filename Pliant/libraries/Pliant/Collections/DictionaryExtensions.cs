using System;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue AddOrGetExisting<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = new TValue();
                dictionary.Add(key, value);
            }

            return value;
        }
        
        public static TValue GetOrReturnNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return default(TValue);
        }
    }
}
