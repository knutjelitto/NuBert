using Pliant.Utilities;

namespace Pliant.Ebnf
{
    public abstract class EbnfFactor : EbnfNode
    {
    }

    public class EbnfFactorIdentifier : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }

        public EbnfFactorIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorIdentifier;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                QualifiedIdentifier.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorIdentifier;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.QualifiedIdentifier.Equals(QualifiedIdentifier);
        }

        public override string ToString()
        {
            return QualifiedIdentifier.ToString();
        }
    }

    public class EbnfFactorLiteral : EbnfFactor
    {
        private readonly int _hashCode;

        public string Value { get; private set; }

        public EbnfFactorLiteral(string value)
        {
            Value = value;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorLiteral;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorLiteral;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Value.Equals(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class EbnfFactorRegex : EbnfFactor
    {
        private readonly int _hashCode;

        public RegularExpressions.Regex Regex { get; private set; }

        public EbnfFactorRegex(RegularExpressions.Regex regex)
        {
            Regex = regex;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorRegex;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Regex.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorRegex;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Regex.Equals(Regex);
        }

        public override string ToString()
        {
            return $"/{Regex}/";
        }
    }

    public class EbnfFactorRepetition : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorRepetition;

        public EbnfFactorRepetition(EbnfExpression expression)
        {
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorRepetition;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }
        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override string ToString()
        {
            return $"{{{Expression}}}";
        }
    }

    public class EbnfFactorOptional : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfFactorOptional(EbnfExpression expression)
        {
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorOptional;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorOptional;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"[{Expression}]";
        }
    }

    public class EbnfFactorGrouping : EbnfFactor
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfFactorGrouping(EbnfExpression expression)
        {
            Expression = expression;
            this._hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType => EbnfNodeType.EbnfFactorGrouping;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var factor = obj as EbnfFactorGrouping;
            if (factor == null)
            {
                return false;
            }

            return factor.NodeType == NodeType
                && factor.Expression.Equals(Expression);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}