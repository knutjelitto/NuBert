using System;

namespace Pliant.Utilities
{
    public struct ValueEquate<T>
        where T : IEquatable<T>
    {
        private readonly T equatable;

        public ValueEquate(T equatable, object obj)
        {
            this.equatable = equatable;
            HashCode = obj.GetHashCode();
        }

        public bool IsEqual(object obj)
        {
            return obj is T other && this.equatable.Equals(other);
        }

        public int HashCode { get; }
    }

    public abstract class ValueEqualityBase<T>
        where T : ValueEqualityBase<T>
    {
        private readonly int hashCode;

        protected ValueEqualityBase(int hashCode)
        {
            this.hashCode = hashCode;
        }

        protected ValueEqualityBase(object obj)
            : this(obj.GetHashCode())
        {
        }

        public abstract bool ThisEquals(T other);

        public override bool Equals(object obj)
        {
            return obj is T other && ThisEquals(other);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}