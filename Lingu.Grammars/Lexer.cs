namespace Lingu.Grammars
{
    public abstract class Lexer
    {
        public Lexer(Provision provision)
        {
            Provision = provision;
        }

        public Provision Provision { get; }

        public abstract bool Match(char @char);

        public abstract bool IsFinal { get; }
    }
}