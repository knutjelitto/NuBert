namespace Lingu.Grammars
{
    public abstract class Lexer
    {
        public Lexer(Terminal terminal)
        {
            Terminal = terminal;
        }

        public Terminal Terminal { get; }
        public Provision Provision => Terminal.Provision;

        public Token MakeToken(string input)
        {
            return new Token(Terminal, Finish(input));
        }

        public abstract bool Match(char @char);

        public abstract bool IsFinal { get; }

        public abstract string Finish(string input);
    }
}