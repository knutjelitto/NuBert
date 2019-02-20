using Pliant.Grammars;

namespace Pliant.Tokens
{
    public class StringLiteralLexeme : LexemeBase<StringLiteralLexerRule>
    {
        public StringLiteralLexeme(StringLiteralLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            this._index = 0;
            this._capture = null;
        }

        public string Literal => ConcreteLexerRule.Literal;

        public override string Value
        {
            get
            {
                if (!IsSubStringAllocated())
                {
                    this._capture = AllocateSubString();
                }

                return this._capture;
            }
        }

        public override bool IsAccepted()
        {
            return this._index >= Literal.Length;
        }

        public override void Reset()
        {
            this._index = 0;
            this._capture = null;
        }

        public override bool Scan(char c)
        {
            if (this._index >= Literal.Length)
            {
                return false;
            }

            if (Literal[this._index] != c)
            {
                return false;
            }

            this._index++;
            return true;
        }

        private string AllocateSubString()
        {
            return Literal.Substring(0, this._index);
        }

        private bool IsSubStringAllocated()
        {
            return this._index == this._capture?.Length;
        }

        private string _capture;
        private int _index;
    }
}