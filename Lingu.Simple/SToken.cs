namespace Lingu.Simple
{
    public class SToken
    {
        public SToken(STerminal from, Input start, Input end)
        {
            From = from;
            Value = start.UpTo(end);
        }

        public STerminal From { get; }
        public string Value { get; }

        public bool IsFrom(STerminal terminal)
        {
            return From.Equals(terminal);
        }
    }
}