using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class ProcessOnceQueue<T> : IQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly Dictionary<T,T> _visited;

        public ProcessOnceQueue()
        {
            this._queue = new Queue<T>();
            this._visited = new Dictionary<T, T>();
        }

        public ProcessOnceQueue(IEnumerable<T> items)
            : this()
        {
            foreach (var item in items)
            {
                Enqueue(item);
            }
        }

        public void Clear()
        {
            this._queue.Clear();
            this._visited.Clear();
        }

        public void Enqueue(T item)
        {
            if (this._visited.ContainsKey(item))
            {
                return;
            }

            this._visited[item] = item;
            this._queue.Enqueue(item);            
        }
        
        public T EnqueueOrGetExisting(T item)
        {
            if (!this._visited.ContainsKey(item))
            {
                this._visited[item] = item;
                this._queue.Enqueue(item);
                return item;
            }
            return this._visited[item];
        }

        public T Dequeue()
        {
            return this._queue.Dequeue();
        }

        public T Peek()
        {
            return this._queue.Peek();
        }

        public T[] ToArray()
        {
            return this._queue.ToArray();
        }

        public void TrimExcess()
        {
            this._queue.TrimExcess();
        }

        public IEnumerable<T> Visited => this._visited.Keys;

        public IEnumerator<T> GetEnumerator()
        {
            return this._queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._queue.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection) this._queue).CopyTo(array, index);
        }

        public int Count => this._queue.Count;

        public object SyncRoot => ((ICollection) this._queue).SyncRoot;

        public bool IsSynchronized => ((ICollection) this._queue).IsSynchronized;
    }
}
