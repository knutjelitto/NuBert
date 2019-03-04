using System.Collections.Generic;
using Pliant.Charts;
using Pliant.Dotted;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class ForestNodeSet
    {
        public ForestNodeSet()
        {
            this._symbolNodes = new Dictionary<int, ISymbolForestNode>();
            this._intermediateNodes = new Dictionary<int, IIntermediateForestNode>();
            this._virtualNodes = new Dictionary<int, VirtualForestNode>();
            this._tokenNodes = new Dictionary<IToken, ITokenForestNode>();
        }

        public void AddNewVirtualNode(VirtualForestNode virtualNode)
        {
            var hash = ComputeHashCode(
                virtualNode.Symbol,
                virtualNode.Origin,
                virtualNode.Location);
            this._virtualNodes.Add(hash, virtualNode);
        }

        public IIntermediateForestNode AddOrGetExistingIntermediateNode(DottedRule dottedRule, int origin, int location)
        {
            var hash = ComputeHashCode(dottedRule, origin, location);

            if (this._intermediateNodes.TryGetValue(hash, out var intermediateNode))
            {
                return intermediateNode;
            }

            intermediateNode = new IntermediateForestNode(dottedRule, origin, location);
            this._intermediateNodes.Add(hash, intermediateNode);
            return intermediateNode;
        }

        public ISymbolForestNode AddOrGetExistingSymbolNode(Symbol symbol, int origin, int location)
        {
            var hash = ComputeHashCode(symbol, origin, location);

            if (this._symbolNodes.TryGetValue(hash, out var symbolNode))
            {
                return symbolNode;
            }

            symbolNode = new SymbolForestNode(symbol, origin, location);
            this._symbolNodes.Add(hash, symbolNode);
            return symbolNode;
        }

        public ITokenForestNode AddOrGetExistingTokenNode(IToken token)
        {
            if (this._tokenNodes.TryGetValue(token, out var tokenNode))
            {
                return tokenNode;
            }

            tokenNode = new TokenForestNode(token, token.Position, token.Value.Length);
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
            var hash = ComputeHashCode(targetState.DottedRule.Production.LeftHandSide, targetState.Origin, location);
            return this._virtualNodes.TryGetValue(hash, out node);
        }

        private static int ComputeHashCode(Symbol symbol, int origin, int location)
        {
            return HashCode.Compute(
                symbol.GetHashCode(),
                origin.GetHashCode(),
                location.GetHashCode());
        }

        private static int ComputeHashCode(DottedRule dottedRule, int origin, int location)
        {
            return HashCode.Compute(
                dottedRule.GetHashCode(),
                origin.GetHashCode(),
                location.GetHashCode());
        }

        private readonly Dictionary<int, IIntermediateForestNode> _intermediateNodes;
        private readonly Dictionary<int, ISymbolForestNode> _symbolNodes;
        private readonly Dictionary<IToken, ITokenForestNode> _tokenNodes;
        private readonly Dictionary<int, VirtualForestNode> _virtualNodes;
    }
}