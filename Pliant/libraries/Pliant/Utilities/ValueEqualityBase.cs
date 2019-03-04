namespace Pliant.Utilities
{
    public abstract class ValueEqualityBase<T>
        where T : ValueEqualityBase<T>
    {
        private int? hashCode;

        protected abstract bool ThisEquals(T other);

        protected abstract object ThisHashCode { get; }

        public override bool Equals(object obj)
        {
            return obj is T other && ThisEquals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.hashCode ?? (this.hashCode = ThisHashCode.GetHashCode()).Value;
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }
    }
}