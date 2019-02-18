using System.Collections.Generic;

namespace Pliant.Tokens
{
    public interface IToken : ITrivia
    {
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}