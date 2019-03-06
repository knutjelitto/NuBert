using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class TokenForestNode : ForestNode, ITokenForestNode
    {
        public TokenForestNode(IToken token)
            : base(token.Position, token.Value.Length)
        {
            Token = token;
            this.hashCode = (NodeType, Origin, Location, Token).GetHashCode();
        }

        public override ForestNodeType NodeType => ForestNodeType.Token;
        public IToken Token { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is TokenForestNode other &&
                   Origin.Equals(other.Origin) &&
                   Location.Equals(other.Location) &&
                   NodeType.Equals(other.NodeType) && 
                   Token.Equals(other.Token);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private readonly int hashCode;
    }
}