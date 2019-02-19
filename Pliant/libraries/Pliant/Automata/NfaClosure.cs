using System;
using System.Diagnostics;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class NfaClosure
    {
        public NfaClosure(NfaStateSet closure, bool isFinal)
        {
            Closure = closure.ToArray();
            State = new DfaState(isFinal);
            this._hashCode = HashCode.Compute(Closure);
        }

        public NfaState[] Closure { get; }

        public DfaState State { get; }

        public override bool Equals(object obj)
        {
#if true
            if (obj is NfaClosure other)
            {
                if (this._hashCode == other._hashCode && !Closure.SequenceEqual(other.Closure))
                {
                    throw new Exception(@"Kacke1");
                }

                if (Closure.SequenceEqual(other.Closure) && this._hashCode != other._hashCode)
                {
                    throw new Exception(@"Kacke2");
                }

                if (this._hashCode != HashCode.Compute(Closure))
                {
                    throw  new Exception(@"Kacke3");
                }

                return Closure.SequenceEqual(other.Closure);
            }

            return false;
#else
                return obj is NfaClosure other && other._hashCode.Equals(this._hashCode);
#endif
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private readonly int _hashCode;
    }
}