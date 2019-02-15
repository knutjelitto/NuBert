using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class UniqueList<T> : IList<T>, IReadOnlyList<T>
    {
        private HashSet<int> _index;
        private readonly List<T> _innerList;

        private const int Threshold = 10;

        public int Count => this._innerList.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => this._innerList[index];
            set { this._innerList[index] = value; }
        }

        public UniqueList()
        {
            this._innerList = new List<T>();
        }

        public UniqueList(int capacity)
        {
            this._innerList = new List<T>(capacity);
        }

        public UniqueList(IEnumerable<T> list)
        {
            this._innerList = new List<T>(list);
            if (HashSetIsMoreEfficient())
            {
                AllocateAndPopulateHashSet();
            }
        }

        public int IndexOf(T item)
        {
            return this._innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InsertUnique(index, item);
        }

        public bool InsertUnique(int index, T item)
        {
            if (HashSetIsMoreEfficient())
            {
                return InsertUniqueUsingHashSet(index, item);
            }

            return InsertUniqueUsingList(index, item);
        }

        private bool InsertUniqueUsingHashSet(int index, T item)
        {
            if (!this._index.Add(item.GetHashCode()))
            {
                return false;
            }

            this._innerList.Insert(index, item);
            return false;
        }

        private bool InsertUniqueUsingList(int index, T item)
        {
            if (this._innerList.Count == 0)
            {
                this._innerList.Insert(index, item);
                return true;
            }
            
            var hashCode = item.GetHashCode();
            for (var i = 0; i < this._innerList.Count; i++)
            {
                var listItem = this._innerList[i];
                if (hashCode.Equals(listItem.GetHashCode()))
                {
                    return false;
                }
            }

            this._innerList.Insert(index, item);
            if (HashSetIsMoreEfficient())
            {
                AllocateAndPopulateHashSet();
            }

            return true;
        }

        public void RemoveAt(int index)
        {
            if (HashSetIsMoreEfficient())
            {
                var item = this._innerList[index];
                this._index.Remove(item.GetHashCode());
            }

            this._innerList.RemoveAt(index);
        }

        public void Add(T item)
        {
            AddUnique(item);
        }

        public bool AddUnique(T item)
        {
            if (HashSetIsMoreEfficient())
            {
                return AddUniqueUsingHashSet(item);
            }

            return AddUniqueUsingList(item);
        }

        private bool AddUniqueUsingHashSet(T item)
        {
            if (!this._index.Add(item.GetHashCode()))
            {
                return false;
            }

            this._innerList.Add(item);
            return true;
        }

        private bool AddUniqueUsingList(T item)
        {
            if (this._innerList.Count == 0)
            {
                this._innerList.Add(item);
                return true;
            }
            var hashCode = item.GetHashCode();
            for(var i=0;i< this._innerList.Count;i++)
            {
                var listItem = this._innerList[i];
                if (hashCode.Equals(listItem.GetHashCode()))
                {
                    return false;
                }
            }

            this._innerList.Add(item);
            if (HashSetIsMoreEfficient())
            {
                AllocateAndPopulateHashSet();
            }

            return true;
        }

        private void AllocateAndPopulateHashSet()
        {
            if (this._index == null)
            {
                this._index = new HashSet<int>();
            }

            if (this._index.Count == this._innerList.Count)
            {
                return;
            }

            for (var i = 0; i < this._innerList.Count; i++)
            {
                this._index.Add(this._innerList[i].GetHashCode());
            }
        }

        public void Clear()
        {
            this._innerList.Clear();
            if(this._index != null)
            {
                this._index.Clear();
            }
        }

        public bool ContainsHash(int hashcode)
        {
            if (HashSetIsMoreEfficient())
            {
                return this._index.Contains(hashcode);
            }

            for (var i = 0; i < this._innerList.Count; i++)
            {
                var item = this._innerList[i];
                if (item.GetHashCode() == hashcode)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T item)
        {
            if (HashSetIsMoreEfficient())
            {
                return this._index.Contains(item.GetHashCode());
            }

            return this._innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (HashSetIsMoreEfficient())
            {
                this._index.Remove(item.GetHashCode());
            }
            return this._innerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        private bool HashSetIsMoreEfficient()
        {
            return this._innerList.Count >= Threshold;
        }

        public override int GetHashCode()
        {
            return this._innerList.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var uniqueList = obj as UniqueList<T>;
            if (uniqueList == null)
            {
                return false;
            }

            return this._innerList.Equals(uniqueList._innerList);
        }

        public T[] ToArray()
        {
            return this._innerList.ToArray();
        }
    }
}
