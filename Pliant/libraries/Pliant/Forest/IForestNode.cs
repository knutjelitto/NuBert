using Pliant.Charts;

namespace Pliant.Forest
{
    public interface IForestNode : IForestNodeVisitable
    {
        int Location { get; }
    }
}