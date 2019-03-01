using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tokens
{
    public class TerminalLexeme : Lexeme
    {
        public TerminalLexeme(TerminalLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            this._captureRendered = false;
            this._isAccepted = false;
            Terminal = lexerRule.Terminal;
        }

        public TerminalLexeme(Terminal terminal, TokenType tokenType, int position)
            : this(new TerminalLexerRule(terminal, tokenType), position)
        {
        }

        public Terminal Terminal { get; }

        public override string Value
        {
            get
            {
                if (!this._isAccepted)
                {
                    return string.Empty;
                }

                if (this._captureRendered)
                {
                    return this._stringCapture;
                }

                this._stringCapture = this._capture.ToString();
                this._captureRendered = true;

                return this._stringCapture;
            }
        }

        public override bool IsAccepted()
        {
            return this._isAccepted;
        }

        public override bool Scan(char c)
        {
            if (IsAccepted())
            {
                return false;
            }

            if (!Terminal.IsMatch(c))
            {
                return false;
            }

            SetCapture(c);
            SetAccepted(true);
            return true;
        }

        private void SetAccepted(bool value)
        {
            this._isAccepted = value;
        }

        private void SetCapture(char value)
        {
            this._capture = value;
        }

        private char _capture;
        private bool _captureRendered;
        private bool _isAccepted;

        private string _stringCapture;
    }
}