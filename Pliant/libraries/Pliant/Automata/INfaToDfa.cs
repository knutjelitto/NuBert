namespace Pliant.Automata
{
    public interface INfaToDfa
    {
        DfaState Transform(Nfa nfa);
    }
}