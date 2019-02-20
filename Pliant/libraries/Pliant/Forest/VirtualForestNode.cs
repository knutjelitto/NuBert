using System.Collections.Generic;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class VirtualForestNode : InternalForestNode, ISymbolForestNode
    {
        public VirtualForestNode(
            int location,
            TransitionState transitionState,
            IForestNode completedParseNode)
            : this(
                location,
                transitionState,
                completedParseNode,
                transitionState.GetTargetState())
        {
        }

        private VirtualForestNode(
            int location,
            TransitionState transitionState,
            IForestNode completedParseNode,
            State targetState)
            : base(targetState.Origin, location)
        {
            this._paths = new List<VirtualForestNodePath>();

            Symbol = targetState.DottedRule.Production.LeftHandSide;
            this._hashCode = ComputeHashCode();
            var path = new VirtualForestNodePath(transitionState, completedParseNode);
            AddUniquePath(path);
        }

        public override IReadOnlyList<IAndForestNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                {
                    LazyLoadChildren();
                }

                return this._children;
            }
        }

        public override ForestNodeType NodeType => ForestNodeType.Symbol;

        public Symbol Symbol { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddUniquePath(VirtualForestNodePath path)
        {
            if (!IsUniquePath(path))
            {
                return;
            }

            if (IsUniqueChildSubTree(path))
            {
                CloneUniqueChildSubTree(path.ForestNode as IInternalForestNode);
            }

            this._paths.Add(path);
        }

        public override bool Equals(object obj)
        {
            return obj is ISymbolForestNode other &&
                   Location == other.Location &&
                   NodeType == other.NodeType &&
                   Origin == other.Origin &&
                   Symbol.Equals(other.Symbol);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        private static bool IsUniqueChildSubTree(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;

            return transitionState.Reduction.ParseNode != null &&
                   Equals(completedParseNode, transitionState.Reduction.ParseNode) && 
                   (completedParseNode.NodeType == ForestNodeType.Intermediate || completedParseNode.NodeType == ForestNodeType.Symbol);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            for (var a = 0; a < internalCompletedParseNode.Children.Count; a++)
            {
                var andNode = internalCompletedParseNode.Children[a];
                var newAndNode = new AndForestNode();
                for (var c = 0; c < andNode.Children.Count; c++)
                {
                    var child = andNode.Children[c];
                    newAndNode.AddChild(child);
                }

                this._children.Add(newAndNode);
            }
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int) NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Symbol.GetHashCode());
        }

        private bool IsUniquePath(VirtualForestNodePath path)
        {
            for (var p = 0; p < this._paths.Count; p++)
            {
                var otherPath = this._paths[p];
                if (path.Equals(otherPath))
                {
                    return false;
                }
            }

            return true;
        }

        private void LazyLoadChildren()
        {
            for (var i = 0; i < this._paths.Count; i++)
            {
                LazyLoadPath(this._paths[i]);
            }
        }

        private void LazyLoadPath(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;
            if (transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualForestNode(Location, transitionState.NextTransition, completedParseNode);

                if (transitionState.Reduction.ParseNode == null)
                {
                    AddUniqueFamily(virtualNode);
                }
                else
                {
                    AddUniqueFamily(virtualNode, transitionState.Reduction.ParseNode);
                }
            }
            else if (transitionState.Reduction.ParseNode != null)
            {
                AddUniqueFamily(completedParseNode, transitionState.Reduction.ParseNode);
            }
            else
            {
                AddUniqueFamily(completedParseNode);
            }
        }

        private bool ShouldLoadChildren()
        {
            return this._children.Count == 0;
        }

        private readonly int _hashCode;
        private readonly List<VirtualForestNodePath> _paths;
    }
}