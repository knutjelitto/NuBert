using System.Collections.Generic;

namespace Lingu.Grammars
{
    public class Terminal : Symbol
    {
        public Terminal(Provision provision)
            : base(provision.Name)
        {
        }
    }
}