using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class UniqueList<T> : IReadOnlyList<T>
    {
        public UniqueList()
        {
            this.list = new List<T>();
            this.hash = new HashSet<T>();
        }

        public T this[int index] => this.list[index];

        public int Count => this.list.Count;

        public bool AddUnique(T item)
        {
            if (!this.hash.Add(item))
            {
                return false;
            }

            this.list.Add(item);
            return true;
        }

        public bool Contains(T item)
        {
            return this.hash.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        private readonly HashSet<T> hash;
        private readonly List<T> list;
    }
}