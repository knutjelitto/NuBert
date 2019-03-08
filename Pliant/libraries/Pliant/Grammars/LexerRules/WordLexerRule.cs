using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class WordLexerRule : DfaLexerRule
    {
        private static readonly DfaState _start;
        private static readonly TokenName _staticTokenType = new TokenName(@"[\w]+");

        static WordLexerRule()
        {
            var start = DfaState.Inner();
            var end = DfaState.Final();

            start.AddTransition(WordTerminal.Instance, end);
            end.AddTransition(WordTerminal.Instance, end);

            _start = start;
        }

        public WordLexerRule()
            : base(_start, _staticTokenType)
        {
        }
    }
}
