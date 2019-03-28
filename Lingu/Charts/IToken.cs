using Lingu.Grammars;

namespace Lingu.Charts
{
    public interface IToken
    {
        bool IsFrom(Terminal terminal);
    }
}