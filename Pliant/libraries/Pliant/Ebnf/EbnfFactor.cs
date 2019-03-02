using Pliant.RegularExpressions;
using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public interface IEbnfFactor : IEbnfNode
    {
    }

    public sealed class EbnfFactorIdentifier : ValueEqualityBase<EbnfFactorIdentifier>, IEbnfFactor
    {
        public EbnfFactorIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
            : base(qualifiedIdentifier.GetHashCode())
        {
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }

        public override bool ThisEquals(EbnfFactorIdentifier other)
        {
            return other.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        public override string ToString()
        {
            return QualifiedIdentifier.ToString();
        }
    }

    public sealed class EbnfFactorLiteral : ValueEqualityBase<EbnfFactorLiteral>, IEbnfFactor
    {
        public EbnfFactorLiteral(string value)
            : base(value.GetHashCode())
        {
            Value = value;
        }

        public string Value { get; }

        public override bool ThisEquals(EbnfFactorLiteral other)
        {
            return Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class EbnfFactorRegex : ValueEqualityBase<EbnfFactorRegex>, IEbnfFactor
    {
        public EbnfFactorRegex(Regex regex)
            : base(regex.GetHashCode())
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        public override bool ThisEquals(EbnfFactorRegex other)
        {
            return Regex.Equals(other.Regex);
        }

        public override string ToString()
        {
            return $"/{Regex}/";
        }
    }

    public sealed class EbnfFactorRepetition : ValueEqualityBase<EbnfFactorRepetition>, IEbnfFactor
    {
        public EbnfFactorRepetition(IEbnfExpression expression)
            : base(expression.GetHashCode())
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        public override bool ThisEquals(EbnfFactorRepetition other)
        {
            return Expression.Equals(other.Expression);
        }

        public override string ToString()
        {
            return $"{{{Expression}}}";
        }
    }

    public sealed class EbnfFactorOptional : ValueEqualityBase<EbnfFactorOptional>, IEbnfFactor
    {
        public EbnfFactorOptional(IEbnfExpression expression)
            : base(expression.GetHashCode())
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        public override bool ThisEquals(EbnfFactorOptional other)
        {
            return other.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"[{Expression}]";
        }
    }

    public class EbnfFactorGrouping : ValueEqualityBase<EbnfFactorGrouping>, IEbnfFactor
    {
        public EbnfFactorGrouping(IEbnfExpression expression)
            : base(expression.GetHashCode())
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        public override bool ThisEquals(EbnfFactorGrouping other)
        {
            return Expression.Equals(other.Expression);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}