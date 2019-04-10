namespace Lingu.Simple
{
    public struct Input
    {
        public Input(string text)
            : this(text, 0)
        {
        }

        private Input(string text, int offset)
        {
            this.text = text;
            this.offset = offset;
        }

        public Input Next => new Input(this.text, this.offset + 1);

        public bool Valid => this.offset < this.text.Length;

        public char Value => this.text[this.offset];

        public string UpTo(Input next)
        {
            return this.text.Substring(this.offset, next.offset - this.offset);
        }

        public static implicit operator bool(Input input) => input.Valid;

        public static implicit operator char(Input input) => input.Value;

        private readonly int offset;
        private readonly string text;
    }
}