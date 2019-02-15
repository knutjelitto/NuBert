using System.Collections;
using System.Collections.Generic;
using Pliant.Forest;

namespace Pliant.Tree
{
    public class ParseTreeEnumerable : IEnumerable<ITreeNode>
    {
        public ParseTreeEnumerable(IInternalForestNode internalForestNode)
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

        private readonly IInternalForestNode _internalForestNode;
    }
}