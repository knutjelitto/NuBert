namespace Pliant.Grammars
{
    public abstract class Symbol
    {
        public bool Is(Symbol other)
        {
            return Equals(other);
        }
    }
}