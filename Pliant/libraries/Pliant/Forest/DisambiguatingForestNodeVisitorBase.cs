namespace Pliant.Forest
{
    public abstract class DisambiguatingForestNodeVisitorBase : ForestNodeVisitorBase
    {
        protected DisambiguatingForestNodeVisitorBase(IForestDisambiguationAlgorithm forestDisambiguationAlgorithm)
        {
            ForestDisambiguationAlgorithm = forestDisambiguationAlgorithm;
        }

        private IForestDisambiguationAlgorithm ForestDisambiguationAlgorithm { get; }

        public override void Visit(IIntermediateForestNode intermediateNode)
        {
            var currentAndNode = ForestDisambiguationAlgorithm.GetCurrentAndNode(intermediateNode);
            Visit(currentAndNode);
        }

        public override void Visit(ITokenForestNode tokenNode)
        {
        }

        public override void Visit(ISymbolForestNode symbolNode)
        {
            var currentAndNode = ForestDisambiguationAlgorithm.GetCurrentAndNode(symbolNode);
            Visit(currentAndNode);
        }
    }
}