namespace Pliant.Grammars
{
    public interface ISymbol
    {
        bool Is(ISymbol other);
    }

    public abstract class Symbol : ISymbol
    {
        public bool Is(ISymbol other)
        {
            return Equals(other);
        }
    }
}