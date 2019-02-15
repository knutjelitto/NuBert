using Pliant.Forest;
using Pliant.Tokens;

namespace Pliant.Tree
{
    public sealed class TokenTreeNode : ITokenTreeNode
    {
        public TokenTreeNode(ITokenForestNode innerNode)
        {
            this._innerNode = innerNode;
        }

        public int Location => this._innerNode.Location;

        public int Origin => this._innerNode.Origin;

        public IToken Token => this._innerNode.Token;

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"{Token.TokenType.Id}({Origin}, {Location}) = {Token.Value}";
        }

        private readonly ITokenForestNode _innerNode;
    }
}