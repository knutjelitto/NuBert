using Lingu.Grammars;

namespace Lingu.Earley
{
    public abstract class EarleyState
    {
        protected EarleyState(DottedRule dottedRule, int origin)
        {
            DottedRule = dottedRule;
            Origin = origin;
        }

        public DottedRule DottedRule { get; }
        public int Origin { get; }

        public Nonterminal Head => DottedRule.Production.Head;

        public abstract bool AddTo(EarleySet set);

        public override string ToString()
        {
            return $"[{Origin}] {DottedRule}";
        }
    }
}