namespace Lingu.Grammars
{
    public class Nonterminal : Symbol
    {
        public Nonterminal(string name)
            : base(name)
        {
        }

        public Nonterminal Head => this;

        public Body Body { get; set; }
    }
}
