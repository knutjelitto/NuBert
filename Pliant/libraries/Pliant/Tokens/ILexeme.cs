using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Tokens
{
    public interface ILexeme : IToken
    {
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}
