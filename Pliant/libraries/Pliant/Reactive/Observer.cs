using System;

namespace Pliant
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        public Observer(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            this._onNext = onNext;
            this._onError = onError;
            this._onCompleted = onCompleted;
        }

        public void OnCompleted()
        {
            if (this._onCompleted != null)
            {
                this._onCompleted();
            }
        }

        public void OnError(Exception error)
        {
            if (this._onError != null)
            {
                this._onError(error);
            }
        }

        public void OnNext(T value)
        {
            if (this._onNext != null)
            {
                this._onNext(value);
            }
        }
    }
}