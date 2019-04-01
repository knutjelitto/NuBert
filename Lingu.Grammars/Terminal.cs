namespace Lingu.Grammars
{
    public class Terminal : Symbol
    {
        public Terminal(Provision provision)
            : base(provision.Name)
        {
            Provision = provision;
        }

        public Provision Provision { get; }

        public Lexer MakeLexer(int offset)
        {
            return Provision.MakeLexer(offset);
        }
    }
}