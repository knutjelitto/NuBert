using Lingu.Grammars;

namespace Lingu.Earley
{
    public interface IToken
    {
        bool IsFrom(Terminal terminal);
    }
}