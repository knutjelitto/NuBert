using System.Collections.Generic;

namespace Lingu.Grammars
{
    public class Body : List<Chain>
    {
        public Body(IEnumerable<Chain> chains)
            : base(chains)
        {
        }
    }
}