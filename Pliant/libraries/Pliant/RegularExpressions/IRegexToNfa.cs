using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public interface IRegexToNfa
    {
        Nfa Transform(Regex regex);
    }
}