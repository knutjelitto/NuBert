using Pliant.Forest;
using Pliant.Tokens;

namespace Pliant.Tree
{
    public sealed class TokenTreeNode : ITokenTreeNode
    {
        public TokenTreeNode(ITokenForestNode innerNode)
        {
            this.innerNode = innerNode;
        }

        public int Location => this.innerNode.Location;

        public IToken Token => this.innerNode.Token;

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"{Token.TokenName}({Location}) = {Token.Value}";
        }

        private readonly ITokenForestNode innerNode;
    }
}