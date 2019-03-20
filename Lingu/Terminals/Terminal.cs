using System.Diagnostics;
using System.Text;
using Lingu.Tools;

namespace Lingu.Terminals
{
    public class Terminal
    {
        private readonly bool not;
        private readonly IntegerSet set;

        public bool Match(char character)
        {
            return this.set.Contains(character);
        }

        public bool NotMatch(char character)
        {
            return !this.set.Contains(character);
        }

        private Terminal(char first, char last)
            : this(false, new IntegerSet((first, last)))
        {
            this.set = new IntegerSet((first, last));
        }

        private Terminal(bool not, IntegerSet set)
        {
            this.not = not;
            this.set = set;
        }

        public static Terminal From(char single)
        {
            return new Terminal(single, single);
        }

        public static Terminal From(char first, char last)
        {
            Debug.Assert(first <= last);
            return new Terminal(first, last);
        }

        public Terminal Not()
        {
            return new Terminal(!this.not, Invert(this.set));
        }

        public Terminal ExceptWith(Terminal other)
        {
            return new Terminal(false, this.set.ExceptWith(other.set));
        }

        public bool Overlaps(Terminal other)
        {
            return this.set.Overlaps(other.set);
        }

        public bool AlmostEquals(Terminal other)
        {
            return this.set.Equals(other.set);
        }

        private static IntegerSet Invert(IntegerSet set)
        {
            return UnicodeSets.Any.Substract(set);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            IntegerSet to;
            if (this.not)
            {
                builder.Append("!(");
                to = Invert(this.set);
            }
            else
            {
                to = this.set;
            }

            foreach (var range in to.GetRanges())
            {
                builder.Append(
                    range.Count == 1
                        ? $"'{(char) range.Min}'"
                        : $"'{(char) range.Min}'-'{(char) range.Max}'");
            }

            if (this.not)
            {
                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}