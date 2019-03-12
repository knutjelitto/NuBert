using System.Diagnostics;

namespace Pliant.Inputs
{
    public struct Cursor
    {
        public Cursor(string text)
            : this(text, 0)
        {
        }

        private Cursor(string text, int index)
        {
            this.text = text;
            this.index = index;
        }

        public bool More => this.index + 1 < this.text.Length;
        public bool Valid => this.index < this.text.Length;
        public int Position => this.index;
        public char Value => this.text[this.index];

        public Cursor Next()
        {
            return new Cursor(this.text, this.index + 1);
        }

        public string Upto(Cursor end)
        {
            Debug.Assert(ReferenceEquals(this.text, end.text));
            Debug.Assert(end.index <= this.text.Length);
            return this.text.Substring(this.index, end.index - this.index);
        }

        public static implicit operator char(Cursor input)
        {
            return input.Value;
        }

        private readonly int index;
        private readonly string text;
    }
}