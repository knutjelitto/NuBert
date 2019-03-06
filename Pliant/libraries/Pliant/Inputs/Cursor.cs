using System.Diagnostics;

namespace Pliant.Inputs
{
    public struct Input
    {
        public Input(string text)
            : this(text, 0)
        {
        }

        private Input(string text, int index)
        {
            this.text = text;
            this.index = index;
        }

        public bool More => this.index + 1 < this.text.Length;
        public bool Valid => this.index < this.text.Length;
        public int Position => this.index;
        public char Value => this.text[this.index];

        public Input Next()
        {
            return new Input(this.text, this.index + 1);
        }

        public string Upto(Input end)
        {
            Debug.Assert(ReferenceEquals(this.text, end.text));
            Debug.Assert(end.index <= this.text.Length);
            return this.text.Substring(this.index, end.index - this.index);
        }

        public static implicit operator char(Input input)
        {
            return input.Value;
        }

        private readonly int index;
        private readonly string text;
    }
}