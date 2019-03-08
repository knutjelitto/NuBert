using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public sealed class TerminalLexerRule : DfaLexerRule
    {
        public TerminalLexerRule(char character)
            : this(new CharacterTerminal(character), new TokenClass(character.ToString()))
        {
        }

        public TerminalLexerRule(Terminal terminal, string tokenTypeId)
            : this(terminal, new TokenClass(tokenTypeId))
        {
        }

        public TerminalLexerRule(Terminal terminal)
            : this(terminal, terminal.ToString())
        {
        }

        public TerminalLexerRule(Terminal terminal, TokenClass tokenClass)
            : base(MakeAutomaton(terminal), tokenClass)
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
            return obj is TerminalLexerRule other &&
                   Terminal.Equals(other.Terminal);
        }

        public override int GetHashCode()
        {
            return (TokenClass, Terminal).GetHashCode();
        }

        public override string ToString()
        {
            return TokenClass.ToString();
        }
    }
}