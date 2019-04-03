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
            Engine = engine;
        }

        public Chart Chart => Engine.Chart;

        public EarleyEngine Engine { get; }

        public Grammar Grammar => Engine.Grammar;

        public bool Next()
        {
            if (this.offset >= this.input.Length)
            {
                return false;
            }

            var matched = Match(this.input[this.offset], CurrentLexers());

            this.offset += 1;
            while (this.offset < this.input.Length)
            {
                var newMatched = Match(this.input[this.offset], matched).ToList();
                if (newMatched.Count == 0)
                {
                    break;
                }
                matched = newMatched;
                this.offset += 1;
            }

            var tokens = matched.Where(lexer => lexer.IsFinal).Select(lexer => lexer.MakeToken(this.input));

            return Engine.Pulse(tokens);
        }

        private IEnumerable<Lexer> CurrentLexers()
        {
            return Chart.Current.Terminals.Select(terminal => terminal.MakeLexer(this.offset));
        }

        private List<Lexer> Match(char @char, IEnumerable<Lexer> lexers)
        {
            IEnumerable<Lexer> match()
            {
                foreach (var lexer in lexers)
                {
                    if (lexer.Match(@char))
                    {
                        yield return lexer;
                    }
                }
            }

            return match().ToList();
        }

        private readonly string input;
        private int offset;
    }
}