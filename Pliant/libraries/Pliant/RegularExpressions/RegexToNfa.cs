using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public abstract class RegexToNfa
    {
        public abstract Nfa Transform(Regex regex);
    }
}