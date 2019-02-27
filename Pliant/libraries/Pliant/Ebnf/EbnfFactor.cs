using Pliant.RegularExpressions;

namespace Pliant.Ebnf
{
    public abstract class EbnfFactor : EbnfNode
    {
    }

    public sealed class EbnfFactorIdentifier : EbnfFactor
    {
        public EbnfFactorIdentifier(EbnfQualifiedIdentifier qualifiedEbnfQualifiedIdentifier)
        {
            QualifiedEbnfQualifiedIdentifier = qualifiedEbnfQualifiedIdentifier;
        }

        public EbnfQualifiedIdentifier QualifiedEbnfQualifiedIdentifier { get; }

        public override int GetHashCode()
        {
            return QualifiedEbnfQualifiedIdentifier.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorIdentifier other &&
                   other.QualifiedEbnfQualifiedIdentifier.Equals(QualifiedEbnfQualifiedIdentifier);
        }

        public override string ToString()
        {
            return QualifiedEbnfQualifiedIdentifier.ToString();
        }
    }

    public sealed class EbnfFactorLiteral : EbnfFactor
    {
        public EbnfFactorLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorLiteral other &&
                   other.Value.Equals(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class EbnfFactorRegex : EbnfFactor
    {
        public EbnfFactorRegex(Regex regex)
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        public override int GetHashCode()
        {
            return Regex.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorRegex other && 
                   other.Regex.Equals(Regex);
        }

        public override string ToString()
        {
            return $"/{Regex}/";
        }
    }

    public sealed class EbnfFactorRepetition : EbnfFactor
    {
        public EbnfFactorRepetition(EbnfExpression expression)
        {
            Expression = expression;
        }

        public EbnfExpression Expression { get; }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorRepetition other && 
                   other.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Expression}}}";
        }
    }

    public sealed class EbnfFactorOptional : EbnfFactor
    {
        public EbnfFactorOptional(EbnfExpression expression)
        {
            Expression = expression;
        }

        public EbnfExpression Expression { get; }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorOptional other && 
                   other.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"[{Expression}]";
        }

    }

    public class EbnfFactorGrouping : EbnfFactor
    {
        public EbnfFactorGrouping(EbnfExpression expression)
        {
            Expression = expression;
        }

        public EbnfExpression Expression { get; }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EbnfFactorGrouping factor && 
                   factor.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}