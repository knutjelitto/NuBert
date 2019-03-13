using System.Collections.Generic;
using System.Linq;
using Lingu.Tools;

namespace Lingu.Automata
{
    public class NfaClosure
    {
        public NfaClosure(int stateNo, NfaState state, NfaState end)
            : this(stateNo, Enumerable.Repeat(state, 1), end)
        {
        }

        public NfaClosure(int stateNo, IEnumerable<NfaState> states, NfaState end)
        {
            var set = new HashSet<NfaState>();
            var isFinal = false;

            foreach (var state in states)
            {
                foreach (var closureState in state.Closure())
                {
                    if (closureState.Equals(end))
                    {
                        isFinal = true;
                    }

                    set.Add(closureState);
                }
            }

            Set = set;
            State = DfaState.Make(stateNo, isFinal);
            this.hashCode = Set.AddedHash();
        }

        public HashSet<NfaState> Set { get; }

        public DfaState State { get; }

        public override bool Equals(object obj)
        {
            return obj is NfaClosure other && Set.SetEquals(other.Set);
        }

        public override int GetHashCode() => this.hashCode;

        private readonly int hashCode;
    }
}
