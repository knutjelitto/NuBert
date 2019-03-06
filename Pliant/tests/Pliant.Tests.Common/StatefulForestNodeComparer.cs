using System.Collections.Generic;
using Pliant.Forest;

namespace Pliant.Tests.Common
{
    public class StatefulForestNodeComparer : IEqualityComparer<IForestNode>
    {
        public StatefulForestNodeComparer()
        {
            this.traversed = new HashSet<IForestNode>();
        }

        public bool Equals(IForestNode firstForestNode, IForestNode secondForestNode)
        {
            if (!this.traversed.Add(firstForestNode))
            {
                return true;
            }

            if (firstForestNode.NodeType != secondForestNode.NodeType)
            {
                return false;
            }

            switch (firstForestNode.NodeType)
            {
                case ForestNodeType.Intermediate:
                    return AreIntermediateNodesEqual(
                        firstForestNode as IIntermediateForestNode,
                        secondForestNode as IIntermediateForestNode);

                case ForestNodeType.Symbol:
                    return AreSymbolNodesEqual(
                        firstForestNode as ISymbolForestNode,
                        secondForestNode as ISymbolForestNode);

                case ForestNodeType.Token:
                    return AreTokenNodesEqual(
                        firstForestNode as ITokenForestNode,
                        secondForestNode as ITokenForestNode);
                default:
                    return false;
            }
        }

        public int GetHashCode(IForestNode obj)
        {
            return obj.GetHashCode();
        }

        private static bool AreTokenNodesEqual(ITokenForestNode firstTokenForestNode, ITokenForestNode secondForestTokenNode)
        {
            return firstTokenForestNode.Token.TokenClass.Id ==
                   secondForestTokenNode.Token.TokenClass.Id
                   && firstTokenForestNode.Token.Value ==
                   secondForestTokenNode.Token.Value;
        }

        private bool AreAndNodesEqual(AndForestNode firstAndNode, AndForestNode secondAndNode)
        {
            if (firstAndNode.Children.Count != secondAndNode.Children.Count)
            {
                return false;
            }

            for (var i = 0; i < firstAndNode.Children.Count; i++)
            {
                if (!Equals(
                        firstAndNode.Children[i],
                        secondAndNode.Children[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreChildNodesEqual(IInternalForestNode firstInternalForestNode, IInternalForestNode secondInternalForestNode)
        {
            if (firstInternalForestNode.Children.Count != secondInternalForestNode.Children.Count)
            {
                return false;
            }

            for (var i = 0; i < firstInternalForestNode.Children.Count; i++)
            {
                if (!AreAndNodesEqual(
                        firstInternalForestNode.Children[i],
                        secondInternalForestNode.Children[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreIntermediateNodesEqual(IIntermediateForestNode firstIntermediateForestNode,
                                               IIntermediateForestNode secondIntermediateForestNode)
        {
            if (!firstIntermediateForestNode.DottedRule.Equals(
                    secondIntermediateForestNode.DottedRule))
            {
                return false;
            }

            return AreChildNodesEqual(firstIntermediateForestNode, secondIntermediateForestNode);
        }

        private bool AreSymbolNodesEqual(ISymbolForestNode firstSymbolForestNode, ISymbolForestNode secondSymbolForestNode)
        {
            if (!firstSymbolForestNode.Symbol.Equals(secondSymbolForestNode.Symbol))
            {
                return false;
            }

            return AreChildNodesEqual(firstSymbolForestNode, secondSymbolForestNode);
        }

        private readonly HashSet<IForestNode> traversed;
    }
}