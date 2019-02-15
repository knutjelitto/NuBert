using System;
using System.Collections.Generic;

namespace Pliant
{
    public abstract class Observable<T> : IObservable<T>
    {
        private readonly IList<IObserver<T>> _observers;

        protected Observable()
        {
            this._observers = new List<IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!this._observers.Contains(observer))
            {
                this._observers.Add(observer);
            }

            return new Unsubscriber(this._observers, observer);
        }

        protected virtual void OnNext(T value)
        {
            foreach (var observer in this._observers)
            {
                observer.OnNext(value);
            }
        }

        protected virtual void OnError(Exception exception)
        {
            foreach (var observer in this._observers)
            {
                observer.OnError(exception);
            }
        }

        protected virtual void Complete()
        {
            foreach (var observer in this._observers)
            {
                if (this._observers.Contains(observer))
                {
                    observer.OnCompleted();
                }
            }

            this._observers.Clear();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly IList<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(IList<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (this._observer != null && this._observers.Contains(this._observer))
                {
                    this._observers.Remove(this._observer);
                }
            }
        }
    }
}