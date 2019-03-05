namespace Pliant.Forest
{
    public abstract class ForestNodeVisitorBase : IForestNodeVisitor
    {

        public abstract void Visit(IIntermediateForestNode intermediateNode);

        public virtual void Visit(AndForestNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                child.Accept(this);
            }
        }

        public abstract void Visit(ISymbolForestNode symbolNode);

        public virtual void Visit(ITokenForestNode tokenNode) { }
    }
}
