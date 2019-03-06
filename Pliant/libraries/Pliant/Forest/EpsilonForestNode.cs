using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class EpsilonForestNode :  ForestNode, ITokenForestNode
    {
        private static readonly TokenClass epsilonTokenType = new TokenClass(string.Empty);

        public EpsilonForestNode(int origin, int location)
            : base(origin, location)
        {
            Token = new VerbatimToken(location, string.Empty, epsilonTokenType);
            this.hashCode = (Origin, Location).GetHashCode();
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
                   Origin.Equals(other.Origin) &&
                   Location.Equals(other.Location);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private readonly int hashCode;
    }
}