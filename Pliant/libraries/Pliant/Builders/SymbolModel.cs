using System;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public abstract class SymbolModel
    {
        protected SymbolModel(Symbol symbol)
        {
            Symbol = symbol;
        }

        public Symbol Symbol { get; }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}