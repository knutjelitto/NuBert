namespace Lingu.Terminals
{
    public abstract class Terminal
    {
        public abstract bool Match(char character);
        public abstract bool NotMatch(char character);

        public NotTerminal Not()
        {
            return new NotTerminal(this);
        }
    }
}