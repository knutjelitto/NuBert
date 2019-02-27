using System.Collections;
using System.Collections.Generic;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tree
{
    public class ParseTreeEnumerator : IEnumerator<ITreeNode>
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

            if (this._forestRoot is IIntermediateForestNode intermediateNode)
            {
                this._visitor.Visit(intermediateNode);
            }
            else if (this._forestRoot is ISymbolForestNode symbolNode)
            {
                this._visitor.Visit(symbolNode);
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
                this.paths = new Dictionary<IInternalForestNode, int>();
                this.nodeStack = new Stack<InternalTreeNodeImpl>();
                this.visited = new HashSet<IInternalForestNode>();
                this.count = 0;
            }

            public ITreeNode Root { get; private set; }

            public void Reset()
            {
                this.paths.Clear();
                this.nodeStack.Clear();
                this.visited.Clear();
                this.count = 0;
                this._lock = null;
            }

            public override void Visit(IIntermediateForestNode intermediateNode)
            {
                if (!this.visited.Add(intermediateNode))
                {
                    return;
                }

                var childIndex = GetOrSetChildIndex(intermediateNode);
                var path = intermediateNode.Children[childIndex];

                Visit(path);
            }

            public override void Visit(ISymbolForestNode symbolNode)
            {
                if (!this.visited.Add(symbolNode))
                {
                    return;
                }

                var childIndex = GetOrSetChildIndex(symbolNode);

                var path = symbolNode.Children[childIndex];
                var internalTreeNode = new InternalTreeNodeImpl(
                    symbolNode.Origin,
                    symbolNode.Location,
                    symbolNode.Symbol as NonTerminal);

                var isRoot = this.nodeStack.Count == 0;
                if (!isRoot)
                {
                    this.nodeStack
                        .Peek()
                        .ReadWriteChildren
                        .Add(internalTreeNode);
                }

                this.nodeStack.Push(internalTreeNode);

                Visit(path);

                var top = this.nodeStack.Pop();

                if (isRoot)
                {
                    if (this.count > 0 && this._lock == null)
                    {
                        Root = null;
                    }
                    else
                    {
                        Root = top;
                    }

                    this.count++;
                    this.visited.Clear();
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
                if (!this.paths.TryGetValue(symbolNode, out var childIndex))
                {
                    this.paths.Add(symbolNode, 0);
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
                    this.paths[symbolNode] = 0;
                    return 0;
                }

                this.paths[symbolNode] = childIndex;
                return childIndex;
            }

            private void VisitToken(int origin, int location, IToken token)
            {
                var tokenTreeNodeImpl = new TokenTreeNodeImpl(
                    origin,
                    location,
                    token);

                var parent = this.nodeStack.Peek();
                parent.ReadWriteChildren.Add(tokenTreeNodeImpl);
            }

            private readonly Stack<InternalTreeNodeImpl> nodeStack;
            private readonly Dictionary<IInternalForestNode, int> paths;
            private readonly HashSet<IInternalForestNode> visited;
            private IInternalForestNode _lock;

            private int count;
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

            public List<ITreeNode> ReadWriteChildren { get; }

            public IReadOnlyList<ITreeNode> Children => ReadWriteChildren;
            public bool Is(QualifiedName name)
            {
                return Symbol.Is(name);
            }

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