using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Forest
{
    public sealed class VirtualForestNode : InternalForestNode, ISymbolForestNode
    {
        public VirtualForestNode(TransitionState transitionState,
                                 int location,
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
            this.paths = new List<VirtualForestNodePath>();

            Symbol = targetState.LeftHandSide;

            this.hashCode = (Origin, Location, Symbol).GetHashCode();
            var path = new VirtualForestNodePath(transitionState, completedParseNode);
            AddUniquePath(path);
        }

        public override IReadOnlyList<AndForestNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                {
                    LazyLoadChildren();
                }

                return this.children;
            }
        }

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

            this.paths.Add(path);
        }

        public override bool Equals(object obj)
        {
            return obj is ISymbolForestNode other &&
                   Location == other.Location &&
                   Origin == other.Origin &&
                   Symbol.Equals(other.Symbol);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        private static bool IsUniqueChildSubTree(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;

            switch (completedParseNode)
            {
                case IntermediateForestNode _:
                    Debug.Assert(true);
                    break;
                case SymbolForestNode _:
                    Debug.Assert(true);
                    break;
                case VirtualForestNode _:
                    Debug.Assert(true);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return transitionState.Reduction.ParseNode != null &&
                   Equals(completedParseNode, transitionState.Reduction.ParseNode) && 
                   (completedParseNode is IIntermediateForestNode || completedParseNode is ISymbolForestNode);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            foreach (var andNode in internalCompletedParseNode.Children)
            {
                var newAndNode = andNode.Clone();

                this.children.Add(newAndNode);
            }
        }

        private bool IsUniquePath(VirtualForestNodePath path)
        {
            foreach (var otherPath in this.paths)
            {
                if (path.Equals(otherPath))
                {
                    return false;
                }
            }

            return true;
        }

        private void LazyLoadChildren()
        {
            foreach (var path in this.paths)
            {
                LazyLoadPath(path);
            }
        }

        private void LazyLoadPath(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;
            if (transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualForestNode(transitionState.NextTransition, Location, completedParseNode);

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
            return this.children.Count == 0;
        }

        private readonly int hashCode;
        private readonly List<VirtualForestNodePath> paths;
    }
}