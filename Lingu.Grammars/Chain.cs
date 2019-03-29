using System;
using System.Collections.Generic;
using System.Text;

namespace Lingu.Grammars
{
    public class Chain : List<Symbol>
    {
        public Chain(IEnumerable<Symbol> symbols)
            : base(symbols)
        {
        }
    }
}
