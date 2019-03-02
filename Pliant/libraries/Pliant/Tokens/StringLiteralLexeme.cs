using Pliant.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tokens
{
    public class StringLiteralLexeme : Lexeme
    {
        public StringLiteralLexeme(StringLiteralLexer lexerRule, int position)
            : base(lexerRule, position)
        {
            this.index = 0;
            this.capture = null;
            Literal = lexerRule.Literal;
        }

        public string Literal { get; }

        public override string Value
        {
            get
            {
                if (!IsSubStringAllocated())
                {
                    this.capture = AllocateSubString();
                }

                return this.capture;
            }
        }

        public override bool IsAccepted()
        {
            return this.index >= Literal.Length;
        }

        public override bool Scan(char c)
        {
            if (this.index >= Literal.Length)
            {
                return false;
            }

            if (Literal[this.index] != c)
            {
                return false;
            }

            this.index++;
            return true;
        }

        private string AllocateSubString()
        {
            return Literal.Substring(0, this.index);
        }

        private bool IsSubStringAllocated()
        {
            return this.index == this.capture?.Length;
        }

        private string capture;
        private int index;
    }
}