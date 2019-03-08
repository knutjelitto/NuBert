using Pliant.Charts;

namespace Pliant.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        DottedRule DottedRule { get; }
    }
}