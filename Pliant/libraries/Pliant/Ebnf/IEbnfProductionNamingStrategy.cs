using Pliant.Grammars;

namespace Pliant.Ebnf
{
    public interface IEbnfProductionNamingStrategy
    {
        NonTerminal GetSymbolForRepetition(EbnfFactorRepetition repetition);
        NonTerminal GetSymbolForOptional(EbnfFactorOptional optional);        
    }
}
