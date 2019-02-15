using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class NamespaceExpression
    {
        public NamespaceExpression(string @namespace)
        {
            Namespace = @namespace;
        }

        public string Namespace { get; }

        public static FullyQualifiedName operator +(NamespaceExpression @namespace, string name)
        {
            return new FullyQualifiedName(@namespace.Namespace, name);
        }

        public static implicit operator NamespaceExpression(string @namespace)
        {
            return new NamespaceExpression(@namespace);
        }
    }
}