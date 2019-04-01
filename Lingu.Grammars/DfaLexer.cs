﻿using Lingu.Automata;
using Lingu.Grammars.Build;

namespace Lingu.Grammars
{
    public class DfaLexer : Lexer
    {
        public DfaLexer(DfaProvision provision, int offset)
            : base(provision)
        {
            Offset = offset;
            Length = 0;
            Current = Dfa.Start;
        }

        public DfaState Current { get; set; }

        public Dfa Dfa => ((DfaProvision) Provision).Dfa;

        public int Length { get; private set; }
        public int Offset { get; }

        public override bool Match(char @char)
        {
            foreach (var transition in Current.Transitions)
            {
                if (transition.Terminal.Match(@char))
                {
                    Current = transition.Target;
                    Length += 1;
                    return true;
                }
            }

            return false;
        }

        public override bool IsFinal => Current.IsFinal;
    }
}