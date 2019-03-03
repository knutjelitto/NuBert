namespace Pliant.RegularExpressions
{
    public abstract class RegexAtom : IRegexNode
    {
    }

    public sealed class RegexAtomAny : RegexAtom
    {
        public override string ToString()
        {
            return Dot;
        }

        public override bool Equals(object obj)
        {
            return obj is RegexAtomAny;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        private const string Dot = ".";
    }

    public class RegexAtomCharacter : RegexAtom
    {
        public RegexAtomCharacter(RegexCharacter character)
        {
            Character = character;
        }

        public RegexCharacter Character { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexAtomCharacter other &&
                   Character.Equals(other.Character);
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    public sealed class RegexAtomExpression : RegexAtom
    {
        public RegexAtomExpression(RegexExpression expression)
        {
            Expression = expression;
        }

        public RegexExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexAtomExpression other &&
                   Expression.Equals(other.Expression);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }

    public sealed class RegexAtomSet : RegexAtom
    {
        public RegexAtomSet(RegexSet set)
        {
            Set = set;
        }

        public RegexSet Set { get; }

        public override bool Equals(object obj)
        {
            return obj is RegexAtomSet other && Set.Equals(other.Set);
        }

        public override int GetHashCode()
        {
            return Set.GetHashCode();
        }

        public override string ToString()
        {
            return Set.ToString();
        }
    }
}