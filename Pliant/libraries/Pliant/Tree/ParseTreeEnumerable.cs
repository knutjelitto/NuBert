using System.Collections;
using System.Collections.Generic;
using Pliant.Forest;

namespace Pliant.Tree
{
    public class ParseTreeEnumerable : IEnumerable<ITreeNode>
    {
        public ParseTreeEnumerable(ISymbolForestNode internalForestNode)
        {
            this._internalForestNode = internalForestNode;
        }

        public IEnumerator<ITreeNode> GetEnumerator()
        {
            return new ParseTreeEnumerator(this._internalForestNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ParseTreeEnumerator(this._internalForestNode);
        }

        private readonly ISymbolForestNode _internalForestNode;
    }
}