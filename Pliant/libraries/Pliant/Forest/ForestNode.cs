namespace Pliant.Forest
{
    public abstract class ForestNode : IForestNode
    {
        protected ForestNode(int location)
        {
            Location = location;
        }

        public int Location { get; }

        public abstract ForestNodeType NodeType { get; }

        public abstract void Accept(IForestNodeVisitor visitor);
    }
}