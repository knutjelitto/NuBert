using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class TokenForestNode : ForestNodeBase, ITokenForestNode
    {
        public TokenForestNode(IToken token, int origin, int location)
            : base(origin, location)
        {
            Token = token;
            this.hashCode = ComputeHashCode();
        }

        public TokenForestNode(string token, int origin, int location)
            : this(new Token(origin, token, new TokenType(token)), origin, location)
        {
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
                   Location.Equals(other.Location) && 
                   NodeType.Equals(other.NodeType) && 
                   Origin.Equals(other.Origin) && 
                   Token.Equals(other.Token);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int) NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Token.GetHashCode());
        }

        private readonly int hashCode;
    }
}