using System;
using Pliant.Forest;
using Pliant.Tokens;

namespace Pliant.Tests.Common.Forest
{
#if true
    public class FakeTokenForestNode : TokenForestNode
    {
        public FakeTokenForestNode(string token, int origin, int location)
            : base(new Token(origin, token, new TokenType(token)), origin, location)
        {
        }
    }
#else
    public class FakeTokenForestNode : ITokenForestNode
    {
        public FakeTokenForestNode(string token, int origin, int location)
            : this(new Token(origin, token, new TokenType(token)), origin, location)
        {
        }

        public FakeTokenForestNode(IToken token, int origin, int location)
        {
            Token = token;
            Origin = origin;
            Location = location;
        }

        public int Location { get; }

        public ForestNodeType NodeType => ForestNodeType.Token;

        public int Origin { get; }

        public IToken Token { get; }

        public void Accept(IForestNodeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
#endif
}