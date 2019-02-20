using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class TerminalLexerRule : LexerRule
    {
        public Terminal Terminal { get; }

        public static readonly LexerRuleType TerminalLexerRuleType = new LexerRuleType("Terminal");

        private readonly int _hashCode;

        public TerminalLexerRule(char character)
            : this(new CharacterTerminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(Terminal terminal, TokenType tokenType)
            : base(TerminalLexerRuleType, tokenType)
        {
            Terminal = terminal;
            this._hashCode = ComputeHashCode(terminal, TerminalLexerRuleType, tokenType);
        }

        public TerminalLexerRule(Terminal terminal, string tokenTypeId)
            : this(terminal, new TokenType(tokenTypeId))
        {
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is TerminalLexerRule terminalLexerRule && 
                   LexerRuleType.Equals(terminalLexerRule.LexerRuleType) && 
                   Terminal.Equals(terminalLexerRule.Terminal);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private static int ComputeHashCode(
            Terminal terminal,
            LexerRuleType terminalLexerRuleType,
            TokenType tokenType)
        {
            return HashCode.Compute(
                terminalLexerRuleType.GetHashCode(),
                tokenType.GetHashCode(),
                terminal.GetHashCode());
        }

        public override bool CanApply(char c)
        {
            return Terminal.IsMatch(c);
        }

        public override ILexeme CreateLexeme(int position)
        {
            return new TerminalLexeme(this, position);
        }
    }
}