using System.Collections.Generic;

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

        private static bool IsMatchedSubTree(IForestNode firstChild, IForestNode secondChild, AndForestNode andNode)
        {
            var firstCompareNode = andNode.First;

            // if first child matches the compare node, continue
            // otherwise return false
            if (!firstChild.Equals(firstCompareNode))
            {
                return false;
            }

            if (secondChild == null)
            {
                return true;
            }

            var secondCompareNode = andNode.Second;

            // return true if the second child matches
            // otherwise return false
            return secondChild.Equals(secondCompareNode);
        }

        private void AddUniqueAndNode(IForestNode child)
        {
            AddUniqueAndNode(child, null);
        }

        private void AddUniqueAndNode(IForestNode firstChild, IForestNode secondChild)
        {
            var childCount = 1 + (secondChild == null ? 0 : 1);

            for (var c = 0; c < this.children.Count; c++)
            {
                var andNode = this.children[c];

                if (andNode.Children.Count != childCount)
                {
                    continue;
                }

                if (IsMatchedSubTree(firstChild, secondChild, andNode))
                {
                    return;
                }
            }

            // not found so return new and node
            var newAndNode = (childCount == 1) ? new AndForestNode(firstChild) : new AndForestNode(firstChild, secondChild) ;

            this.children.Add(newAndNode);
        }

        protected readonly List<AndForestNode> children;
    }
}