namespace Pliant.Utilities
{
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