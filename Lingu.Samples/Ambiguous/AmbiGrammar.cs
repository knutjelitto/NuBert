using Lingu.Automata;
using Lingu.Grammars;
using Lingu.Grammars.Build;

namespace Lingu.Samples.Expr
{
    // ReSharper disable once UnusedMember.Global
    public class AmbiGrammar
    {
        public static Grammar Create()
        {
            TerminalExpr dot = ".";

            RuleExpr Start = "start";
            RuleExpr Dot = "dot";
            RuleExpr DotDot = "dotdot";
            RuleExpr Dot_DotDot = "dot-dotdot";
            RuleExpr DotDot_Dot = "dotdot-dot";

            Start.Body =
                (Dot_DotDot + DotDot_Dot) |
                (DotDot_Dot + Dot_DotDot);

            Dot.Body = dot;

            DotDot.Body = Dot + Dot;

            Dot_DotDot.Body = Dot + DotDot;

            DotDot_Dot.Body = DotDot + Dot;

            return new GrammarBuilder().From(Start);
        }
    }
}
