namespace Pliant.Forest
{
    public interface IForestNodeVisitor
    {
        void Visit(ITerminalForestNode node);

        void Visit(ISymbolForestNode node);

        void Visit(IIntermediateForestNode node);

        void Visit(AndForestNode andNode);

        void Visit(ITokenForestNode tokenNode);
    }
}