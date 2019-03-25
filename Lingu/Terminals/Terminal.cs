using System.Diagnostics;
using System.Text;
using Lingu.Tools;

namespace Lingu.Terminals
{
    public class Terminal
    {
        private Terminal(char first, char last)
            : this(false, new IntegerSet((first, last)))
        {
            Set = new IntegerSet((first, last));
        }

        private Terminal(bool not, IntegerSet set)
        {
            this.not = not;
            Set = set;
        }

        public IntegerSet Set { get; }

        public static Terminal From(char single)
        {
            return new Terminal(single, single);
        }

        public static Terminal From(char first, char last)
        {
            Debug.Assert(first <= last);
            return new Terminal(first, last);
        }

        public static Terminal From(IntegerSet set)
        {
            return new Terminal(false, set);
        }

        public override bool Equals(object obj)
        {
            return obj is Terminal other && Set.Equals(other.Set);
        }

        public override int GetHashCode()
        {
            return Set.GetHashCode();
        }

        public bool Match(char character)
        {
            return Set.Contains(character);
        }

        public Terminal Not()
        {
            return new Terminal(!this.not, Invert(Set));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            IntegerSet to;
            if (this.not)
            {
                builder.Append("!(");
                to = Invert(Set);
            }
            else
            {
                to = Set;
            }

            builder.Append(to);

            if (this.not)
            {
                builder.Append(")");
            }

            return builder.ToString();
        }

        private static IntegerSet Invert(IntegerSet set)
        {
            return UnicodeSets.Any.Substract(set);
        }

        private readonly bool not;
    }
}