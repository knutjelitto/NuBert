namespace Pliant.Tokens
{
    public abstract class Trivia : ITrivia
    {
        protected Trivia(int position, string value, TokenName tokenType)
        {
            Value = value;
            Position = position;
            TokenName = tokenType;
        }

        public int Position { get; }
        public TokenName TokenName { get; }
        public virtual string Value { get; }
    }
}