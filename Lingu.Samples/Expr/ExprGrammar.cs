using Lingu.Automata;
using Lingu.Grammars;
using Lingu.Grammars.Build;

namespace Lingu.Samples.Expr
{
    // ReSharper disable once UnusedMember.Global
    public class ExprGrammar
    {
        public static Grammar Create()
        {
            RuleExpr Expr = "Expr";
            RuleExpr Sum = "Sum";
            RuleExpr Product = "Product";
            RuleExpr Factor = "Factor";
            RuleExpr Primary = "Primary";
            RuleExpr Number = "Number";

            Expr.Body = Sum;

            Sum.Body = 
                (Sum + '+' + Product) |
                (Sum + '-' + Product) |
                Product;

            Product.Body =
                (Product + '*' + Factor) |
                (Product + '/' + Factor) |
                Factor;

            Factor.Body = Primary;

            Primary.Body =
                ('(' + Sum + ')') |
                Number;

            Number.Body = NumberTerminal();

            return new GrammarBuilder().From(Expr);
        }

        private static TerminalExpr NumberTerminal()
        {
            // [-+]?[0-9]+

            var digit = (Nfa)('0', '9');
            var sign = (Nfa)'+' | '-';

            var number = sign.Opt + digit.Plus;

            return TerminalExpr.From(DfaProvision.From("number", number));
        }
    }
}
