using Pliant.Grammars;

namespace Pliant.Ebnf
{
    public interface IEbnfProductionNamingStrategy
    {
        INonTerminal GetSymbolForRepetition(EbnfFactorRepetition repetition);
        INonTerminal GetSymbolForOptional(EbnfFactorOptional optional);        
    }
}
