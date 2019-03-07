using System;
using System.Collections.Generic;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Tree
{
    public sealed class InternalTreeNode : IInternalTreeNode
    {
        public InternalTreeNode(
            ISymbolForestNode internalNode,
            IForestDisambiguationAlgorithm stateManager)
        {
            this.disambiguationAlgorithm = stateManager;
            this.internalNode = internalNode;
            this.children = null;
            SetSymbol(this.internalNode);
        }

        public InternalTreeNode(
            ISymbolForestNode internalNode)
            : this(internalNode, new SelectFirstChildDisambiguationAlgorithm())
        {
        }

        public int Origin => this.internalNode.Origin;

        public int Location => this.internalNode.Location;

        public NonTerminal Symbol { get; private set; }

        public IReadOnlyList<ITreeNode> Children
        {
            get
            {
                if (this.children == null)
                {
                    this.children = new List<ITreeNode>();
                    var andNode = this.disambiguationAlgorithm.GetCurrentAndNode(this.internalNode);
                    LazyLoadChildren(andNode);
                }

                return this.children;
            }
        }

        public bool Is(QualifiedName name)
        {
            return Symbol.Is(name);
        }

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void SetSymbol(IInternalForestNode node)
        {
            switch (node)
            {
                case ISymbolForestNode symbol:
                    Symbol = symbol.Symbol as NonTerminal;
                    break;

                case IIntermediateForestNode intermediate:
                    Symbol = intermediate.DottedRule.Production.LeftHandSide;
                    break;
            }
        }

        private void LazyLoadChildren(AndForestNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                switch (child)
                {
                    // skip intermediate nodes by enumerating children only
                    case IIntermediateForestNode intermediateNode:
                        var currentAndNode = this.disambiguationAlgorithm.GetCurrentAndNode(intermediateNode);
                        LazyLoadChildren(currentAndNode);
                        break;

                    // create a internal tree node for symbol forest nodes
                    case ISymbolForestNode symbolNode:
                        this.children.Add(new InternalTreeNode(symbolNode, this.disambiguationAlgorithm));
                        break;

                    // create a tree token node for token forest nodes
                    case ITokenForestNode tokenNode:
                        this.children.Add(new TokenTreeNode(tokenNode));
                        break;

                    default:
                        throw new Exception("Unrecognized NodeType");
                }
            }
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        private List<ITreeNode> children;
        private readonly IForestDisambiguationAlgorithm disambiguationAlgorithm;
        private readonly ISymbolForestNode internalNode;
    }
}