using System.Collections.Generic;

namespace Pliant.Collections
{
    public class ProcessOnceQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly Dictionary<T,T> _visited;

        public ProcessOnceQueue()
        {
            this._queue = new Queue<T>();
            this._visited = new Dictionary<T, T>();
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
            if (this._visited.TryGetValue(item, out var found))
            {
                return found;
            }

            this._visited.Add(item, item);
            this._queue.Enqueue(item);
            return item;
        }

        public T Dequeue()
        {
            return this._queue.Dequeue();
        }

        public IEnumerable<T> Visited => this._visited.Keys;

        public IEnumerator<T> GetEnumerator()
        {
            return this._queue.GetEnumerator();
        }

        public int Count => this._queue.Count;
    }
}
