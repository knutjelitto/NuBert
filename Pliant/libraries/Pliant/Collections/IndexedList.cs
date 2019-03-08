using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class IndexedList<T> : IList<T>, IReadOnlyList<T>
    {
        public IndexedList()
        {
            this._indexOfDictionary = new Dictionary<T, int>();
            this._innerList = new List<T>();
        }

        public T this[int index]
        {
            get => this._innerList[index];
            set => Insert(index, value);
        }

        public int Count => this._innerList.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (this._indexOfDictionary.ContainsKey(item))
            {
                return;
            }

            this._indexOfDictionary[item] = this._innerList.Count;
            this._innerList.Add(item);
        }

        public void Clear()
        {
            this._indexOfDictionary.Clear();
            this._innerList.Clear();
        }

        public bool Contains(T item)
        {
            return this._indexOfDictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._innerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            if (!this._indexOfDictionary.TryGetValue(item, out var value))
            {
                return -1;
            }

            return value;
        }

        public void Insert(int index, T item)
        {
            var oldIndex = IndexOf(item);
            if (oldIndex >= 0)
            {
                this._innerList.RemoveAt(oldIndex);
                if (oldIndex < index)
                {
                    index -= 1;
                }
            }

            this._innerList.Insert(index, item);
            ShiftLeft(Math.Min(index, oldIndex >= 0 ? oldIndex : index));
        }

        public bool Remove(T item)
        {
            var indexOf = IndexOf(item);
            if (indexOf < 0)
            {
                return false;
            }

            if (!this._indexOfDictionary.Remove(item))
            {
                return false;
            }

            this._innerList.RemoveAt(indexOf);
            ShiftLeft(indexOf);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = this._innerList[index];
            this._innerList.RemoveAt(index);
            this._indexOfDictionary.Remove(item);
            ShiftLeft(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        private void ShiftLeft(int index)
        {
            for (var i = index; i < this._innerList.Count; i++)
            {
                this._indexOfDictionary[this._innerList[i]] = i;
            }
        }

        private readonly Dictionary<T, int> _indexOfDictionary;
        private readonly List<T> _innerList;
    }
}