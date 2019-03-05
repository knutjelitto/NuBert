using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Grammars
{
    public sealed class TerminalLexer : DfaLexer
    {
        public TerminalLexer(char character)
            : this(new CharacterTerminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexer(Terminal terminal, string tokenTypeId)
            : this(terminal, new TokenType(tokenTypeId))
        {
        }

        public TerminalLexer(Terminal terminal)
            : this(terminal, terminal.ToString())
        {
        }

        public TerminalLexer(Terminal terminal, TokenType tokenType)
            : base(MakeAutomaton(terminal), tokenType)
        {
            Terminal = terminal;
        }

        private static DfaState MakeAutomaton(Terminal terminal)
        {
            return DfaState.Inner().AddTransition(terminal, DfaState.Final());
        }

        public Terminal Terminal { get; }

        public override bool CanApply(char c)
        {
            return Terminal.IsMatch(c);
        }

#if false
        public override Lexeme CreateLexeme(int position)
        {
            return new TerminalLexeme(this, position);
        }
#endif

        public override bool Equals(object obj)
        {
            return obj is TerminalLexer other &&
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