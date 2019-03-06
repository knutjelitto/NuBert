using Pliant.Automata;
using Pliant.Terminals;

namespace Pliant.LexerRules
{
    public class NumberLexerRule : DfaLexerRule
    {
        static NumberLexerRule()
        {
            var states = new[]
            {
                DfaState.Inner(),
                DfaState.Inner(),
                DfaState.Final(),
                DfaState.Inner(),
                DfaState.Final(),
            };

            var zeroThroughNine = new RangeTerminal('0', '9');

            states[0].AddTransition(new CharacterTerminal('.'), states[3]);
            states[0].AddTransition(new SetTerminal("+-"), states[1]);
            states[0].AddTransition(zeroThroughNine, states[2]);

            states[1].AddTransition(new CharacterTerminal('.'), states[3]);
            states[1].AddTransition(zeroThroughNine, states[2]);

            states[2].AddTransition(zeroThroughNine, states[2]);
            states[2].AddTransition(new CharacterTerminal('.'), states[3]);

            states[3].AddTransition(zeroThroughNine, states[4]);

            states[4].AddTransition(zeroThroughNine, states[4]);

            startState = states[0];
        }

        public NumberLexerRule()
            : base(startState, pattern)
        {
        }

        private const string pattern = @"[-+]?[0-9]*[.]?[0-9]+";
        private static readonly DfaState startState;
    }
}