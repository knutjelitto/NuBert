using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class EpsilonForestNode :  ForestNode, ITokenForestNode
    {
        private static readonly TokenClass epsilonTokenType = new TokenClass(string.Empty);

        public EpsilonForestNode(int location)
            : base(location)
        {
            Token = new VerbatimToken(location, string.Empty, epsilonTokenType);
            this.hashCode = (Location).GetHashCode();
        }

        public override ForestNodeType NodeType => ForestNodeType.Token;
        public IToken Token { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is EpsilonForestNode other &&
                   Location.Equals(other.Location);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private readonly int hashCode;
    }
}