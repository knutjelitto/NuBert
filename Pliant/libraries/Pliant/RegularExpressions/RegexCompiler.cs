using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public class RegexCompiler
    {
        private readonly IRegexToNfa _regexToNfa;
        private readonly INfaToDfa _nfaToDfa;

        public RegexCompiler()
            : this(
                new ThompsonConstructionAlgorithm(),
                new SubsetConstructionAlgorithm())
        {}

        public RegexCompiler(
            IRegexToNfa regexToNfa,
            INfaToDfa nfaToDfa)
        {
            this._regexToNfa = regexToNfa;
            this._nfaToDfa = nfaToDfa;
        }

        public DfaState Compile(Regex regex)
        {
            var nfa = this._regexToNfa.Transform(regex);
            var dfa = this._nfaToDfa.Transform(nfa);
            return dfa;
        }
    }
}