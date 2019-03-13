using System;
using System.Collections.Generic;
using System.Text;

namespace Lingu.Terminals
{
    public class SetTerminal : Terminal
    {
        private readonly Terminal[] terminals;

        public SetTerminal(params Terminal[] terminals)
        {
            this.terminals = terminals;
        }

        public override bool Match(char current)
        {
            foreach (var terminal in this.terminals)
            {
                if (terminal.Match(current))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool NotMatch(char character)
        {
            foreach (var terminal in this.terminals)
            {
                if (terminal.Match(character))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join<Terminal>("|", this.terminals);
        }
    }
}
