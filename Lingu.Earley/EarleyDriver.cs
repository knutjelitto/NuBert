using System.Collections.Generic;
using System.Linq;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class EarleyDriver
    {
        public EarleyDriver(EarleyEngine engine, string input)
        {
            this.input = input;
            this.offset = 0;
            this.matches = new HashSet<int>();
            Engine = engine;
        }

        public Chart Chart => Engine.Chart;

        public EarleyEngine Engine { get; }

        public Grammar Grammar => Engine.Grammar;

        public bool Next()
        {
            if (this.lexers == null)
            {
                this.lexers = (from terminal in Chart.Current.Terminals
                               select terminal.MakeLexer(this.offset)).ToList();
                this.matches.Clear();
            }

            var more = false;

            for (var i = 0; i < this.lexers.Count; i++)
            {
                var lexer = this.lexers[i];
                if (lexer.Match(this.input[this.offset]))
                {
                    this.matches.Add(i);
                    more = true;
                }
                else
                {
                    
                }
            }

            return false;
        }

        private readonly string input;

        private List<Lexer> lexers;
        private HashSet<int> matches;
        private readonly int offset;
    }
}