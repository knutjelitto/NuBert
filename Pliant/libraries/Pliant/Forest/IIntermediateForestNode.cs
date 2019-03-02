using Pliant.Dotted;

namespace Pliant.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        DottedRule DottedRule { get; }
    }
}