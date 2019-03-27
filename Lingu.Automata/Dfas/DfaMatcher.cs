namespace Lingu.Automata
{
    public class DfaMatcher
    {
        public DfaMatcher(Dfa dfa)
        {
            Dfa = dfa;
            State = Dfa.Start;
        }

        public Dfa Dfa { get; }

        public DfaState State { get; private set; }

        public bool FullMatch(string characters)
        {
            State = Dfa.Start;

            foreach (var character in characters)
            {
                var matched = false;

                foreach (var transition in State.Transitions)
                {
                    if (transition.Terminal.Match(character))
                    {
                        State = transition.Target;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return false;
                }
            }

            return State.IsFinal;
        }
    }
}