using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class TerminalForestNode : ForestNode, ITerminalForestNode
    {
        public TerminalForestNode(char capture, int origin, int location)
            : base(origin, location)
        {
            Capture = capture;
            this._hashCode = ComputeHashCode();
        }

        public char Capture { get; }

        public override ForestNodeType NodeType => ForestNodeType.Terminal;

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is TerminalForestNode other && 
                   Location == other.Location && 
                   NodeType == other.NodeType && 
                   Origin == other.Origin && 
                   Capture.Equals(other.Capture);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"({(Capture == '\0' ? "null" : Capture.ToString())}, {Origin}, {Location})";
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Capture.GetHashCode());
        }

        private readonly int _hashCode;
    }
}