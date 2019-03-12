using Pliant.Automata;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public sealed class TerminalLexerRule : DfaLexerRule
    {
        public TerminalLexerRule(char character)
            : this(new CharacterTerminal(character), new TokenName(character.ToString()))
        {
        }

        public TerminalLexerRule(AtomTerminal terminal, string tokenTypeId)
            : this(terminal, new TokenName(tokenTypeId))
        {
        }

        public TerminalLexerRule(AtomTerminal terminal)
            : this(terminal, terminal.ToString())
        {
        }

        public TerminalLexerRule(AtomTerminal terminal, TokenName tokenName)
            : base(MakeAutomaton(terminal), tokenName)
        {
            Terminal = terminal;
        }

        private static DfaState MakeAutomaton(AtomTerminal terminal)
        {
            return DfaState.Inner().AddTransition(terminal, DfaState.Final());
        }

        public AtomTerminal Terminal { get; }

        public override bool CanApply(char c)
        {
            return Terminal.CanApply(c);
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
            return (TokenName, Terminal).GetHashCode();
        }

        public override string ToString()
        {
            return TokenName.ToString();
        }
    }
}