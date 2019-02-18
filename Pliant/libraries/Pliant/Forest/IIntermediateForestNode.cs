using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        DottedRule DottedRule { get; }
    }
}