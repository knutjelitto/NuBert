namespace Pliant.Forest
{
    public abstract class ForestNode : IForestNode
    {
        protected ForestNode(int origin, int location)
        {
            Origin = origin;
            Location = location;
        }

        public int Location { get; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; }

        public abstract void Accept(IForestNodeVisitor visitor);
    }
}