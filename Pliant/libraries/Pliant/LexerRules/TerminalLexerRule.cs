using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Grammars
{
    public sealed class TerminalLexerRule : Lexer
    {
        public TerminalLexerRule(char character)
            : this(new CharacterTerminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(Terminal terminal, TokenType tokenType)
            : base(tokenType)
        {
            Terminal = terminal;
        }

        public TerminalLexerRule(Terminal terminal, string tokenTypeId)
            : this(terminal, new TokenType(tokenTypeId))
        {
        }

        public TerminalLexerRule(Terminal terminal)
            : this(terminal, terminal.ToString())
        {
        }

        public Terminal Terminal { get; }

        public override bool CanApply(char c)
        {
            return Terminal.IsMatch(c);
        }

        public override Lexeme CreateLexeme(int position)
        {
            return new TerminalLexeme(this, position);
        }

        public override bool Equals(object obj)
        {
            return obj is TerminalLexerRule other &&
                   Terminal.Equals(other.Terminal);
        }

        public override int GetHashCode()
        {
            return (TokenType, Terminal).GetHashCode();
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }
    }
}