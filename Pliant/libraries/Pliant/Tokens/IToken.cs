using System.Collections.Generic;

namespace Pliant.Tokens
{
    public interface IToken
    {
        int Position { get; }
        string Value { get; }
        TokenType TokenType { get; }
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}