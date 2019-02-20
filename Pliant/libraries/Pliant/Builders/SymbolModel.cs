using System;
using Pliant.Grammars;

namespace Pliant.Builders
{
    public abstract class SymbolModel
    {
        public abstract Symbol Symbol { get; }

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