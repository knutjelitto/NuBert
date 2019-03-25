﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Lingu.Automata
{
    public partial class Nfa
    {
        public Nfa(NfaState start, NfaState end)
        {
            Start = start;
            End = end;
        }

        public NfaState End { get; }
        public NfaState Start { get; }

        public Dfa ToDfa()
        {
            return DfaConverter.Convert(this);
        }

        public void Dump()
        {
            Dump(Console.Out);
        }

        public void Dump(TextWriter writer)
        {
            new NfaPlumber(this).Dump(writer);
        }

        public Nfa Clone()
        {
            var map = new Dictionary<NfaState, NfaState>();

            NfaState Map(NfaState state)
            {
                if (!map.TryGetValue(state, out var mapped))
                {
                    mapped = new NfaState();
                    map.Add(state, mapped);

                    foreach (var transition in state.TerminalTransitions)
                    {
                        mapped.Add(transition.Terminal, Map(transition.Target));
                    }
                    foreach (var transition in state.EpsilonTransitions)
                    {
                        mapped.Add(Map(transition.Target));
                    }
                }

                return mapped;
            }

            return new Nfa(Map(Start), Map(End));
        }
    }
}