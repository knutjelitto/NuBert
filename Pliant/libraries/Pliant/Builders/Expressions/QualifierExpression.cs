using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class QualifierExpression
    {
        private QualifierExpression(string qualifier)
        {
            Qualifier = qualifier;
        }

        public string Qualifier { get; }

        public static QualifiedName operator +(QualifierExpression qualifier, string name)
        {
            return new QualifiedName(qualifier.Qualifier, name);
        }

        public static implicit operator QualifierExpression(string qualifier)
        {
            return new QualifierExpression(qualifier);
        }
    }
}