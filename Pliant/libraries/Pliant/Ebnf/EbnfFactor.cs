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
        {
            QualifiedIdentifier = qualifiedIdentifier;
        }

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; }

        protected override bool ThisEquals(EbnfFactorIdentifier other)
        {
            return other.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        protected override object ThisHashCode => QualifiedIdentifier;

        public override string ToString()
        {
            return QualifiedIdentifier.ToString();
        }
    }

    public sealed class EbnfFactorLiteral : ValueEqualityBase<EbnfFactorLiteral>, IEbnfFactor
    {
        public EbnfFactorLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override bool ThisEquals(EbnfFactorLiteral other)
        {
            return Value.Equals(other.Value);
        }

        protected override object ThisHashCode => Value;

        public override string ToString()
        {
            return Value;
        }
    }

    public class EbnfFactorRegex : ValueEqualityBase<EbnfFactorRegex>, IEbnfFactor
    {
        public EbnfFactorRegex(Regex regex)
        {
            Regex = regex;
        }

        public Regex Regex { get; }

        protected override bool ThisEquals(EbnfFactorRegex other)
        {
            return Regex.Equals(other.Regex);
        }

        protected override object ThisHashCode => Regex;

        public override string ToString()
        {
            return $"/{Regex}/";
        }
    }

    public sealed class EbnfFactorRepetition : ValueEqualityBase<EbnfFactorRepetition>, IEbnfFactor
    {
        public EbnfFactorRepetition(IEbnfExpression expression)
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        protected override bool ThisEquals(EbnfFactorRepetition other)
        {
            return Expression.Equals(other.Expression);
        }

        protected override object ThisHashCode => Expression;

        public override string ToString()
        {
            return $"{{{Expression}}}";
        }
    }

    public sealed class EbnfFactorOptional : ValueEqualityBase<EbnfFactorOptional>, IEbnfFactor
    {
        public EbnfFactorOptional(IEbnfExpression expression)
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        protected override bool ThisEquals(EbnfFactorOptional other)
        {
            return other.Expression.Equals(Expression);
        }

        protected override object ThisHashCode => Expression;

        public override string ToString()
        {
            return $"[{Expression}]";
        }
    }

    public class EbnfFactorGrouping : ValueEqualityBase<EbnfFactorGrouping>, IEbnfFactor
    {
        public EbnfFactorGrouping(IEbnfExpression expression)
        {
            Expression = expression;
        }

        public IEbnfExpression Expression { get; }

        protected override bool ThisEquals(EbnfFactorGrouping other)
        {
            return Expression.Equals(other.Expression);
        }

        protected override object ThisHashCode => Expression;

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}