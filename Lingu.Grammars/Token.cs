namespace Lingu.Grammars
{
    public class Token : IToken
    {
        public Token(Terminal terminal, string value)
        {
            Terminal = terminal;
            Value = value;
        }

        public Terminal Terminal { get; }
        public string Value { get; }

        public bool IsFrom(Terminal terminal)
        {
            return Terminal.Equals(terminal);
        }

        public override string ToString()
        {
            return $"{Terminal}:{Value}";
        }
    }
}