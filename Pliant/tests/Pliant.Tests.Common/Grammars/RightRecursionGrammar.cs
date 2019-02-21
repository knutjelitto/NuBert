using Pliant.Builders.Expressions;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Grammars
{
    public class RightRecursionGrammar : GrammarWrapper
    {
        private static readonly IGrammar grammar;

        static RightRecursionGrammar()
        {
            ProductionExpression A = "A";
            A.Rule =
                ('a' + A)
                | (Expr)null;

            grammar = new GrammarExpression(A, new[] { A })
                .ToGrammar();
        }

        public RightRecursionGrammar() : base(grammar)
        {
        }
    }
}
