using Lingu.Grammars;

namespace Lingu.Charts
{
    public abstract class EarleyItem
    {
        public EarleyItem(DottedRule dotted, int origin)
        {
            Dotted = dotted;
            Origin = origin;
        }

        public DottedRule Dotted { get; }
        public int Origin { get; }

        public Nonterminal Head => Dotted.Production.Head;

        public abstract bool AddTo(EarleySet set);

        public override string ToString()
        {
            return $"[{Origin}] {Dotted}";
        }
    }
}