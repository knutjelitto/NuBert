namespace Lingu.Grammars
{
    public interface IToken
    {
        bool IsFrom(Terminal terminal);
    }
}