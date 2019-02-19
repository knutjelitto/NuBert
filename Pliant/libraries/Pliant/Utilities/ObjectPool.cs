﻿using System;
using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal class ObjectPool<T> where T : class
    {
        private readonly Queue<T> _queue;
        private readonly ObjectPoolFactory _factory;

        internal delegate T ObjectPoolFactory();
        
        internal ObjectPool(int size, ObjectPoolFactory factory)
        {
            this._factory = factory;
            this._queue = new Queue<T>(size);
        }

        internal ObjectPool(ObjectPoolFactory factory)
            : this(20, factory)
        {
        }
        
        internal T Allocate()
        {
            if (this._queue.Count == 0)
            {
                return CreateInstance();
            }

            return this._queue.Dequeue();
        }

        private T CreateInstance()
        {
            return this._factory();
        }

        internal void Free(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._queue.Enqueue(value);
        }        
    }
}
