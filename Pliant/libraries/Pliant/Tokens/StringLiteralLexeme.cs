using Pliant.Grammars;

namespace Pliant.Tokens
{
    public class StringLiteralLexeme : LexemeBase<StringLiteralLexerRule>, ILexeme
    {
        private string _capture;
        private int _index;
        
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
        
        public StringLiteralLexeme(StringLiteralLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            this._index = 0;
            this._capture = null;
        }

        private bool IsSubStringAllocated()
        {
            if (this._capture == null)
            {
                return false;
            }

            return this._index == this._capture.Length;
        }

        private string AllocateSubString()
        {
            return Literal.Substring(0, this._index);
        }

        public override bool IsAccepted()
        {
            return this._index >= Literal.Length;
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

        public override void Reset()
        {
            this._index = 0;
            this._capture = null;
        }      
    }
}