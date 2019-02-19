using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public interface IEarleySet
    {
        IReadOnlyList<NormalState> Predictions { get; }

        IReadOnlyList<NormalState> Scans { get; }

        IReadOnlyList<NormalState> Completions { get; }

        IReadOnlyList<TransitionState> Transitions { get; }

        bool Enqueue(State state);

        int Location { get; }

        TransitionState FindTransitionState(ISymbol searchSymbol);
        
        NormalState FindSourceState(ISymbol searchSymbol);
    }
}