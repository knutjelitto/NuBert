using System.Collections;
using System.Collections.Generic;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tree
{
    public class ParseTreeEnumerator : IParseTreeEnumerator
    {
        public ParseTreeEnumerator(IInternalForestNode forestRoot)
        {
            this._forestRoot = forestRoot;
            this._status = ParseTreeEnumeratorState.New;
            this._visitor = new ForestNodeVisitorImpl();
        }

        public ITreeNode Current
        {
            get
            {
                switch (this._status)
                {
                    case ParseTreeEnumeratorState.Current:
                        return this._visitor.Root;

                    default:
                        return null;
                }
            }
        }

        public void Dispose()
        {
            this._visitor = null;
            this._forestRoot = null;
        }

        public bool MoveNext()
        {
            if (this._status == ParseTreeEnumeratorState.Done)
            {
                return false;
            }

            if (this._forestRoot.NodeType == ForestNodeType.Intermediate)
            {
                this._visitor.Visit(this._forestRoot as IIntermediateForestNode);
            }
            else if (this._forestRoot.NodeType == ForestNodeType.Symbol)
            {
                this._visitor.Visit(this._forestRoot as ISymbolForestNode);
            }

            if (this._visitor.Root == null)
            {
                this._status = ParseTreeEnumeratorState.Done;
                return false;
            }

            this._status = ParseTreeEnumeratorState.Current;
            return true;
        }

        public void Reset()
        {
            this._status = ParseTreeEnumeratorState.New;
            this._visitor.Reset();
        }

        object IEnumerator.Current => Current;
        private IInternalForestNode _forestRoot;
        private ParseTreeEnumeratorState _status;
        private ForestNodeVisitorImpl _visitor;

        #region  not sortable (modify ReSharper template to catch these cases)

        private enum ParseTreeEnumeratorState
        {
            New,
            Current,
            Done
        }

        private class ForestNodeVisitorImpl : ForestNodeVisitorBase
        {
            public ForestNodeVisitorImpl()
            {
                this._paths = new Dictionary<IInternalForestNode, int>();
                this._nodeStack = new Stack<InternalTreeNodeImpl>();
                this._visited = new HashSet<IInternalForestNode>();
                this._count = 0;
            }

            public ITreeNode Root { get; private set; }

            public void Reset()
            {
                this._paths.Clear();
                this._nodeStack.Clear();
                this._visited.Clear();
                this._count = 0;
                this._lock = null;
            }

            public override void Visit(IIntermediateForestNode intermediateNode)
            {
                if (!this._visited.Add(intermediateNode))
                {
                    return;
                }

                var childIndex = GetOrSetChildIndex(intermediateNode);
                var path = intermediateNode.Children[childIndex];

                Visit(path);
            }

            public override void Visit(ISymbolForestNode symbolNode)
            {
                if (!this._visited.Add(symbolNode))
                {
                    return;
                }

                var childIndex = GetOrSetChildIndex(symbolNode);

                var path = symbolNode.Children[childIndex];
                var internalTreeNode = new InternalTreeNodeImpl(
                    symbolNode.Origin,
                    symbolNode.Location,
                    symbolNode.Symbol as NonTerminal);

                var isRoot = this._nodeStack.Count == 0;
                if (!isRoot)
                {
                    this._nodeStack
                        .Peek()
                        .ReadWriteChildren
                        .Add(internalTreeNode);
                }

                this._nodeStack.Push(internalTreeNode);

                Visit(path);

                var top = this._nodeStack.Pop();

                if (isRoot)
                {
                    if (this._count > 0 && this._lock == null)
                    {
                        Root = null;
                    }
                    else
                    {
                        Root = top;
                    }

                    this._count++;
                    this._visited.Clear();
                }
            }

            public override void Visit(ITerminalForestNode terminalNode)
            {
                var token = new Token(terminalNode.Origin,
                    terminalNode.Capture.ToString(),
                                      new TokenType(terminalNode.ToString()));
                VisitToken(terminalNode.Origin, terminalNode.Location, token);
            }

            public override void Visit(ITokenForestNode tokenNode)
            {
                VisitToken(tokenNode.Origin, tokenNode.Location, tokenNode.Token);
            }

            private int GetOrSetChildIndex(IInternalForestNode symbolNode)
            {
                if (!this._paths.TryGetValue(symbolNode, out var childIndex))
                {
                    this._paths.Add(symbolNode, 0);
                    return childIndex;
                }

                var isLocked = !ReferenceEquals(null, this._lock);
                if (!isLocked)
                {
                    this._lock = symbolNode;
                }

                var isCurrentNodeLocked = ReferenceEquals(this._lock, symbolNode);
                if (!isCurrentNodeLocked)
                {
                    return childIndex;
                }

                childIndex++;
                if (childIndex >= symbolNode.Children.Count)
                {
                    this._lock = null;
                    this._paths[symbolNode] = 0;
                    return 0;
                }

                this._paths[symbolNode] = childIndex;
                return childIndex;
            }

            private void VisitToken(int origin, int location, IToken token)
            {
                var tokenTreeNodeImpl = new TokenTreeNodeImpl(
                    origin,
                    location,
                    token);

                var parent = this._nodeStack.Peek();
                parent.ReadWriteChildren.Add(tokenTreeNodeImpl);
            }

            private int _count;
            private IInternalForestNode _lock;

            private readonly Stack<InternalTreeNodeImpl> _nodeStack;
            private readonly Dictionary<IInternalForestNode, int> _paths;
            private readonly HashSet<IInternalForestNode> _visited;
        }

        private abstract class TreeNodeImpl : ITreeNode
        {
            protected TreeNodeImpl(int origin, int location)
            {
                Origin = origin;
                Location = location;
            }

            public int Location { get; }

            public int Origin { get; }

            public abstract void Accept(ITreeNodeVisitor visitor);
        }

        private class InternalTreeNodeImpl : TreeNodeImpl, IInternalTreeNode
        {
            public InternalTreeNodeImpl(int origin, int location, NonTerminal symbol)
                : base(origin, location)
            {
                Symbol = symbol;
                ReadWriteChildren = new List<ITreeNode>();
            }

            public IReadOnlyList<ITreeNode> Children => ReadWriteChildren;
            public List<ITreeNode> ReadWriteChildren { get; }

            public NonTerminal Symbol { get; }

            public override void Accept(ITreeNodeVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        private class TokenTreeNodeImpl : TreeNodeImpl, ITokenTreeNode
        {
            public TokenTreeNodeImpl(int origin, int location, IToken token)
                : base(origin, location)
            {
                Token = token;
            }

            public IToken Token { get; }

            public override void Accept(ITreeNodeVisitor visitor)
            {
                visitor.Visit(this);
            }
        }

        #endregion
    }
}