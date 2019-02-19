using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public class RegexCompiler
    {
        public RegexCompiler()
            : this(new ThompsonConstructionAlgorithm(), new SubsetConstructionAlgorithm())
        {
        }

        private RegexCompiler(IRegexToNfa regexToNfa, INfaToDfa nfaToDfa)
        {
            this.regexToNfa = regexToNfa;
            this.nfaToDfa = nfaToDfa;
        }

        public DfaState Compile(Regex regex)
        {
            var nfa = this.regexToNfa.Transform(regex);
            var dfa = this.nfaToDfa.Transform(nfa);
            return dfa;
        }

        private readonly INfaToDfa nfaToDfa;
        private readonly IRegexToNfa regexToNfa;
    }
}