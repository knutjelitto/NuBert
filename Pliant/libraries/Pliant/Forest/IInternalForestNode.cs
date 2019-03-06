using System.Collections.Generic;

namespace Pliant.Forest
{
    /// <summary>
    /// Represents a Disjuncion of IAndNodes
    /// </summary>
    public interface IInternalForestNode : IForestNode
    {
        int Origin { get; }

        IReadOnlyList<AndForestNode> Children { get; }

        void AddUniqueFamily(IForestNode trigger);

        void AddUniqueFamily(IForestNode trigger, IForestNode source);
    }
}