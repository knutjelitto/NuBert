namespace Lingu.Terminals
{
    public class SingleTerminal : Terminal
    {
        public SingleTerminal(char character)
        {
            this.character = character;
        }

        public override bool Match(char current)
        {
            return this.character == current;
        }

        public override bool NotMatch(char current)
        {
            return this.character != current;
        }

        public override string ToString()
        {
            return $"'{this.character}'";
        }

        private readonly char character;
    }
}