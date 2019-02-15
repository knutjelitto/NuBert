using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    /// <summary>
    /// Uses a list for items when the count of items is low. Uses a dictionary when the count of items is above the threshold.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FastLookupDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _innerDictionary;
        private readonly List<KeyValuePair<TKey, TValue>> _innerList;
        private const int Threshold = 4;
        private int _count;

        public FastLookupDictionary()
        {
            this._innerDictionary = new Dictionary<TKey, TValue>();
            this._innerList = new List<KeyValuePair<TKey, TValue>>();
        }

        public KeyValuePair<TKey, TValue> GetByIndex(int index)
        {
            return this._innerList[index];
        }

        private TValue Get(TKey key)
        {
            if (DictionaryIsMoreEfficient())
            {
                return GetValueFromDictionary(key);
            }

            return GetValueFromList(key);
        }

        private bool DictionaryIsMoreEfficient()
        {
            return this._count > Threshold;
        }

        private bool TryGetValueFromDictionary(TKey key, out TValue value)
        {
            return this._innerDictionary.TryGetValue(key, out value);
        }

        private TValue GetValueFromDictionary(TKey key)
        {
            if (this._innerDictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return default(TValue);
        }

        private bool TryGetValueFromList(TKey key, out TValue value)
        {
            value = default(TValue);
            var hashCode = key.GetHashCode();
            for (var i = 0; i < this._innerList.Count; i++)
            {
                var keyValuePair = this._innerList[i];
                if (!hashCode.Equals(keyValuePair.Key.GetHashCode()))
                {
                    continue;
                }

                value = keyValuePair.Value;
                return true;
            }
            return false;
        }

        private TValue GetValueFromList(TKey key)
        {
            var keyHashCode = key.GetHashCode();
            for (var i = 0; i < this._innerList.Count; i++)
            {
                var keyValuePair = this._innerList[i];
                if (keyHashCode == keyValuePair.Key.GetHashCode())
                {
                    return keyValuePair.Value;
                }
            }
            return default(TValue);
        }

        private void Set(TKey key, TValue value)
        {
            if (DictionaryIsMoreEfficient())
            {
                SetValueInDictionary(key, value);
            }
            else
            {
                SetValueInList(key, value);
            }

            if (AtThreshold())
            {
                TransferItemsToDictionary();
            }

            this._count++;
        }


        private void SetValueInDictionary(TKey key, TValue value)
        {
            this._innerDictionary.Add(key, value);
        }

        private void SetValueInList(TKey key, TValue value)
        {
            var keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
            this._innerList.Add(keyValuePair);
        }

        private bool AtThreshold()
        {
            return this._count == Threshold;
        }

        private void TransferItemsToDictionary()
        {
            for (var i = 0; i < this._innerList.Count; i++)
            {
                var keyValuePair = this._innerList[i];
                this._innerDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public TValue this[TKey key]
        {
            get => Get(key);
            set { Set(key, value); }
        }

        public int Count
        {
            get
            {
                if (DictionaryIsMoreEfficient())
                {
                    return this._innerDictionary.Count;
                }

                return this._innerList.Count;
            }
        }

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys
        {
            get
            {
                if (DictionaryIsMoreEfficient())
                {
                    return this._innerDictionary.Keys;
                }

                var list = new List<TKey>();
                for (var i = 0; i < this._innerList.Count; i++)
                {
                    var keyValuePair = this._innerList[i];
                    list.Add(keyValuePair.Key);
                }
                return list;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (DictionaryIsMoreEfficient())
                {
                    return this._innerDictionary.Values;
                }

                var list = new List<TValue>();
                for (var i = 0; i < this._innerList.Count; i++)
                {
                    var keyValuePair = this._innerList[i];
                    list.Add(keyValuePair.Value);
                }
                return list;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Set(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            Set(key, value);
        }

        public void Clear()
        {
            this._innerDictionary.Clear();
            this._innerList.Clear();
            this._count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            if (DictionaryIsMoreEfficient())
            {
                return this._innerDictionary.ContainsKey(key);
            }

            var hashCode = key.GetHashCode();
            for (var i = 0; i < this._innerList.Count; i++)
            {
                if (hashCode.Equals(this._innerList[i].Key.GetHashCode()))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (DictionaryIsMoreEfficient())
            {
                return this._innerDictionary.GetEnumerator();
            }

            return this._innerList.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (DictionaryIsMoreEfficient())
            {
                return TryGetValueFromDictionary(key, out value);
            }

            return TryGetValueFromList(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (DictionaryIsMoreEfficient())
            {
                return this._innerDictionary.GetEnumerator();
            }

            return this._innerList.GetEnumerator();
        }
    }
}
