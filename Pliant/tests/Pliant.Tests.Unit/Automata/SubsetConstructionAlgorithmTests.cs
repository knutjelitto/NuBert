using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class SubsetConstructionAlgorithmTests
    {
        [TestMethod]
        public void SubsetConstructionAlgorithmShouldConvertComplexNfaToDfa()
        {
            var a = new CharacterTerminal('a');
            var b = new CharacterTerminal('b');
            var c = new CharacterTerminal('c');
            var states = CreateStates(4);

            states[0].AddTransition(a, states[1]);
            states[0].AddTransition(c, states[3]);
            states[1].AddEpsilon(states[0]);
            states[1].AddTransition(b, states[2]);
            states[2].AddTransition(a, states[1]);
            states[3].AddTransition(c, states[2]);
            states[3].AddEpsilon(states[2]);

            var dfa_0 = ConvertNfaToDfa(new Nfa(states[0], states[2]));

            Assert.IsNotNull(dfa_0);

            DfaTransition transition_0_01 = null;
            DfaTransition transition_0_23 = null;

            var count = 0;
            foreach (var transition in dfa_0.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.CanApply('a'))
                {
                    transition_0_01 = transition;
                }
                else if (terminal.CanApply('c'))
                {
                    transition_0_23 = transition;
                }

                count++;
            }

            Assert.AreEqual(2, count);

            var dfa_01 = transition_0_01.Target;
            DfaTransition transition_01_01 = null;
            DfaTransition transition_01_23 = null;
            DfaTransition transition_01_2 = null;

            count = 0;
            foreach (var transition in dfa_01.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.CanApply('a'))
                {
                    transition_01_01 = transition;
                }
                else if (terminal.CanApply('b'))
                {
                    transition_01_2 = transition;
                }
                else if (terminal.CanApply('c'))
                {
                    transition_01_23 = transition;
                }

                count++;
            }

            Assert.AreEqual(3, count);

            var dfa_23 = transition_0_23.Target;
            DfaTransition transition_23_01 = null;
            DfaTransition transition_23_2 = null;

            count = 0;
            foreach (var transition in dfa_23.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.CanApply('a'))
                {
                    transition_23_01 = transition;
                }
                else if (terminal.CanApply('c'))
                {
                    transition_23_2 = transition;
                }

                count++;
            }

            Assert.AreEqual(2, count);

            var dfa_2 = transition_23_2.Target;
            DfaTransition transition_2_01 = null;
            count = 0;
            foreach (var transition in dfa_2.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.CanApply('a'))
                {
                    transition_2_01 = transition;
                }

                count++;
            }

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SubsetConstructionAlgorithmShouldConvertSingleCharacterNfaToEquivalentDfa()
        {
            var a = new CharacterTerminal('a');
            var states = CreateStates(2);

            states[0].AddTransition(a, states[1]);

            var nfa = new Nfa(states[0], states[1]);
            var dfa = ConvertNfaToDfa(nfa);

            Assert.IsNotNull(dfa);
        }

        private static NfaState[] CreateStates(int count)
        {
            var states = new NfaState[count];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new EntityNfaState(i);
            }

            return states;
        }

        private DfaState ConvertNfaToDfa(Nfa nfa)
        {
            return new SubsetConstructionAlgorithm().Transform(nfa);
        }

        #region  not sortable (modify ReSharper template to catch these cases)

        private class EntityNfaState : NfaState
        {
            public EntityNfaState(int id)
            {
                Id = id;
            }

            public int Id { get; }

            public override bool Equals(object obj)
            {
                return obj is EntityNfaState other && other.Id.Equals(Id);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override string ToString()
            {
                return "NfaState: " + Id.ToString();
            }
        }

        #endregion
    }
}