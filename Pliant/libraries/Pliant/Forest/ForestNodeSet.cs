using System.Collections.Generic;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Forest
{
    public class ForestNodeSet
    {
        public ForestNodeSet()
        {
            this._symbolNodes = new Dictionary<(NonTerminal, int, int), ISymbolForestNode>();
            this._intermediateNodes = new Dictionary<(DottedRule, int, int), IIntermediateForestNode>();
            this._virtualNodes = new Dictionary<(Symbol, int, int), VirtualForestNode>();
            this._tokenNodes = new Dictionary<IToken, TokenForestNode>();
        }

        public void AddNewVirtualNode(VirtualForestNode virtualNode)
        {
            var key = (virtualNode.Symbol, virtualNode.Origin, virtualNode.Location);
            this._virtualNodes.Add(key, virtualNode);
        }

        public IIntermediateForestNode AddOrGetExistingIntermediateNode(DottedRule dottedRule, int origin, int location)
        {
            var key = (dottedRule, origin, location);

            if (this._intermediateNodes.TryGetValue(key, out var intermediateNode))
            {
                return intermediateNode;
            }

            intermediateNode = new IntermediateForestNode(dottedRule, origin, location);
            this._intermediateNodes.Add(key, intermediateNode);
            return intermediateNode;
        }

        public ISymbolForestNode AddOrGetExistingSymbolNode(NonTerminal symbol, int origin, int location)
        {
            var key = (symbol, origin, location);

            if (this._symbolNodes.TryGetValue(key, out var symbolNode))
            {
                return symbolNode;
            }

            symbolNode = new SymbolForestNode(symbol, origin, location);
            this._symbolNodes.Add(key, symbolNode);
            return symbolNode;
        }

        public ITokenForestNode AddOrGetExistingTokenNode(IToken token)
        {
            if (this._tokenNodes.TryGetValue(token, out var tokenNode))
            {
                return tokenNode;
            }

            tokenNode = new TokenForestNode(token);
            this._tokenNodes.Add(token, tokenNode);
            return tokenNode;
        }

        public void Clear()
        {
            this._symbolNodes.Clear();
            this._intermediateNodes.Clear();
            this._virtualNodes.Clear();
            this._tokenNodes.Clear();
        }

        public bool TryGetExistingVirtualNode(
            int location,
            TransitionState transitionState,
            out VirtualForestNode node)
        {
            var targetState = transitionState.GetTargetState();
            var key = (targetState.LeftHandSide, targetState.Origin, location);
            return this._virtualNodes.TryGetValue(key, out node);
        }

        private readonly Dictionary<(DottedRule, int, int), IIntermediateForestNode> _intermediateNodes;
        private readonly Dictionary<(NonTerminal, int, int), ISymbolForestNode> _symbolNodes;
        private readonly Dictionary<IToken, TokenForestNode> _tokenNodes;
        private readonly Dictionary<(Symbol, int, int), VirtualForestNode> _virtualNodes;
    }
}