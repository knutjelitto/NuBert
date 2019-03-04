namespace Pliant.Forest
{
    public interface IForestDisambiguationAlgorithm
    {
        AndForestNode GetCurrentAndNode(IInternalForestNode internalNode);
    }
}