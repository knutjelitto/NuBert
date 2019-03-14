namespace Lingu.Terminals
{
    public class NotTerminal : Terminal
    {
        private readonly Terminal negate;

        public NotTerminal(Terminal negate)
        {
            this.negate = negate;
        }

        public override bool Match(char character)
        {
            return this.negate.NotMatch(character);
        }

        public override bool NotMatch(char character)
        {
            return this.negate.Match(character);
        }

        public override int CompareTo(object obj)
        {
            return CompareTo(obj as NotTerminal);
        }

        public override bool Equals(object obj)
        {
            return obj is NotTerminal other && this.negate.Equals(other.negate);
        }

        public override int GetHashCode()
        {
            return this.negate.GetHashCode();
        }

        public override string ToString()
        {
            return $"!({this.negate})";
        }
    }
}
