using System;
using System.Collections.Generic;
using System.Text;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public interface IToken
    {
        bool IsFrom(Terminal terminal);
    }
}
