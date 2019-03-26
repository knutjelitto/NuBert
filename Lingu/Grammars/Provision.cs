namespace Lingu.Grammars
{
    public abstract class Provision
    {
        protected Provision(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}