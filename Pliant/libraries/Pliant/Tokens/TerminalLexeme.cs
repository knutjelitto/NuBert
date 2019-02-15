using Pliant.Grammars;

namespace Pliant.Tokens
{
    public class TerminalLexeme : LexemeBase<ITerminalLexerRule>, ILexeme
    {
        public ITerminal Terminal => ConcreteLexerRule.Terminal;

        string _stringCapture;
        char _capture;
        bool _captureRendered;
        bool _isAccepted;
                
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

        public TerminalLexeme(ITerminalLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            this._captureRendered = false;
            this._isAccepted = false;
        }

        public TerminalLexeme(ITerminal terminal, TokenType tokenType, int position)
            : this(new TerminalLexerRule(terminal, tokenType), position)
        {
        }

        public override void Reset()
        {
            this._captureRendered = false;
            this._isAccepted = false;
        }
                
        public override bool IsAccepted()
        {
            return this._isAccepted;
        }

        void SetAccepted(bool value)
        {
            this._isAccepted = value;
        }

        void SetCapture(char value)
        {
            this._capture = value;
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

    }
}