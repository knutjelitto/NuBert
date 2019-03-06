namespace Pliant.Tokens
{
    public interface ITrivia
    {
        int Position { get; }
        string Value { get; }
        TokenClass TokenClass { get; }
    }
}