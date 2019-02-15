using Pliant.Forest;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public sealed class InternalTreeNode : IInternalTreeNode
    {
        private readonly IForestDisambiguationAlgorithm _disambiguationAlgorithm;
        private readonly IInternalForestNode _internalNode;
        private readonly List<ITreeNode> _children;

        public int Origin => this._internalNode.Origin;

        public int Location => this._internalNode.Location;

        public INonTerminal Symbol { get; private set; }

        public InternalTreeNode(
            IInternalForestNode internalNode,
            IForestDisambiguationAlgorithm stateManager)
        {
            this._disambiguationAlgorithm = stateManager;
            this._internalNode = internalNode;
            this._children = new List<ITreeNode>();
            SetSymbol(this._internalNode);
        }

        public InternalTreeNode(
            IInternalForestNode internalNode)
            : this(internalNode, new SelectFirstChildDisambiguationAlgorithm())
        {
        }
        
        private void SetSymbol(IInternalForestNode node)
        {
            switch (node.NodeType)
            {
                case ForestNodeType.Symbol:
                    Symbol = (node as ISymbolForestNode).Symbol as INonTerminal;
                    break;

                case ForestNodeType.Intermediate:
                    Symbol = (node as IIntermediateForestNode).DottedRule.Production.LeftHandSide;
                    break;
            }
        }

        public IReadOnlyList<ITreeNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                {
                    var andNode = this._disambiguationAlgorithm.GetCurrentAndNode(this._internalNode);
                    LazyLoadChildren(andNode);
                }
                return this._children;
            }
        }

        private void LazyLoadChildren(IAndForestNode andNode)
        {
            for (var c = 0; c < andNode.Children.Count; c++)
            {
                var child = andNode.Children[c];
                switch (child.NodeType)
                {
                    // skip intermediate nodes by enumerating children only
                    case ForestNodeType.Intermediate:
                        var intermediateNode = child as IIntermediateForestNode;
                        var currentAndNode = this._disambiguationAlgorithm.GetCurrentAndNode(intermediateNode);
                        LazyLoadChildren(currentAndNode);
                        break;

                    // create a internal tree node for symbol forest nodes
                    case ForestNodeType.Symbol:
                        var symbolNode = child as ISymbolForestNode;
                        this._children.Add(new InternalTreeNode(symbolNode, this._disambiguationAlgorithm));
                        break;
                        
                    // create a tree token node for token forest nodes
                    case ForestNodeType.Token:
                        this._children.Add(new TokenTreeNode(child as ITokenForestNode));
                        break;

                    default:
                        throw new Exception("Unrecognized NodeType");
                }
            }            
        }

        private bool ShouldLoadChildren()
        {
            return this._children.Count == 0;
        }

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }
    }
}