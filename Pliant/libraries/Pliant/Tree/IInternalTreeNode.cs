using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public interface IInternalTreeNode : ITreeNode
    {
        NonTerminal Symbol { get; }
        IReadOnlyList<ITreeNode> Children { get; }

        bool Is(QualifiedName name);
    }
}