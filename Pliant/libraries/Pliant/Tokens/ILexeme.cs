using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexeme : IToken, ITrivia
    {
        bool Scan(char c);

        bool IsAccepted();
             
        ILexerRule LexerRule { get; }

        void AddTrailingTrivia(ITrivia trivia);

        void AddLeadingTrivia(ITrivia trivia);

        void Reset();
    }
}