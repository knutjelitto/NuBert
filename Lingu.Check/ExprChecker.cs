using System;
using System.Collections.Generic;
using System.Text;
using Lingu.Grammars;
using Lingu.Samples.Expr;

namespace Lingu.Check
{
    public class ExprChecker
    {
        public void Check()
        {
            var grammar = ExprGrammar.Create();
            var plumber = new GrammarPlumber(grammar);
            plumber.Dump(Console.Out);
        }
    }
}
