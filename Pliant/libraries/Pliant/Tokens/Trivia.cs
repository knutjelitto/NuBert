namespace Pliant.Tokens
{
    public abstract class Trivia : ITrivia
    {
        protected Trivia(int position, string value, TokenClass tokenType)
        {
            Value = value;
            Position = position;
            TokenClass = tokenType;
        }

        public int Position { get; }
        public TokenClass TokenClass { get; }
        public virtual string Value { get; }
    }
}