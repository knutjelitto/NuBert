using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexeme : IToken
    {
        bool Scan(char c);

        bool IsAccepted();

        void AddTrailingTrivia(ITrivia trivia);

        void AddLeadingTrivia(ITrivia trivia);

        void Reset();
    }
}