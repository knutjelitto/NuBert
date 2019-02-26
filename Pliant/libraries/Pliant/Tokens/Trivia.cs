namespace Pliant.Tokens
{
    public abstract class Trivia : ITrivia
    {
        protected Trivia(int position, string value, TokenType tokenType)
        {
            Value = value;
            Position = position;
            TokenType = tokenType;
        }

        public int Position { get; }
        public TokenType TokenType { get; }
        public virtual string Value { get; }
    }
}