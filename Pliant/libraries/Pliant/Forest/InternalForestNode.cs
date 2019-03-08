using System.Collections.Generic;
using System.Diagnostics;

namespace Pliant.Forest
{
    public abstract class InternalForestNode : ForestNode, IInternalForestNode
    {
        protected InternalForestNode(int origin, int location, params AndForestNode[] children)
            : base(location)
        {
            Origin = origin;
            this.children = new List<AndForestNode>(children);
        }

        public int Origin { get; }

        public virtual IReadOnlyList<AndForestNode> Children => this.children;

        public void AddUniqueFamily(IForestNode trigger)
        {
            Debug.Assert(!(this is IntermediateForestNode) || Children.Count == 0);
            AddUniqueAndNode(trigger);
        }

        public void AddUniqueFamily(IForestNode trigger, IForestNode source)
        {
            if (ReferenceEquals(source, this))
            {
                source = Children[0].First;
            }

            AddUniqueAndNode(source, trigger);
        }

        private static bool IsMatchedSubTree(AndForestNode andNode, IForestNode first, IForestNode second)
        {
            return first.Equals(andNode.First) && (second == null || second.Equals(andNode.Second));
        }

        private void AddUniqueAndNode(IForestNode first, IForestNode second = null)
        {
            var childCount = 1 + (second == null ? 0 : 1);

            foreach (var andNode in this.children)
            {
                if (andNode.Children.Count != childCount)
                {
                    continue;
                }

                if (IsMatchedSubTree(andNode, first, second))
                {
                    return;
                }
            }

            // not found so add new and node
            var newAndNode = AndForestNode.Make(first, second);

            Debug.Assert(!(this is IntermediateForestNode) || this.children.Count == 0);
            this.children.Add(newAndNode);
        }

        protected readonly List<AndForestNode> children;
    }
}