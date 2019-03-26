using System.Collections.Generic;

namespace Lingu.Commons
{
    public class UniqueQueue<T>
    {
        private readonly Dictionary<T, T> set;
        private readonly Queue<T> queue;

        public UniqueQueue()
        {
            this.set = new Dictionary<T, T>();
            this.queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            if (!this.set.ContainsKey(item))
            {
                this.set.Add(item, item);
                this.queue.Enqueue(item);
            }
        }

        public bool Enqueue(T item, out T already)
        {
            if (this.set.TryGetValue(item, out already))
            {
                return false;
            }

            this.set.Add(item, item);
            this.queue.Enqueue(item);
            already = item;

            return true;
        }

        public T Dequeue()
        {
            return this.queue.Dequeue();
        }

        public IEnumerable<T> Seen => this.set.Keys;

        public int Count => this.queue.Count;
    }
}
