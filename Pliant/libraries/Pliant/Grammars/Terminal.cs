using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class Terminal : Symbol
    {
        public abstract bool CanApply(char character);
    }
}