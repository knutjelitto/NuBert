namespace Lingu.Grammars
{
    public abstract class Symbol
    {
        protected Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool IsNullable { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
