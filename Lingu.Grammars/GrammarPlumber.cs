using System.IO;
using System.Linq;

namespace Lingu.Grammars
{
    public class GrammarPlumber
    {
        public GrammarPlumber(Grammar grammar)
        {
            this.grammar = grammar;
        }

        public void Dump(TextWriter writer)
        {
            var symbols = string.Join(", ", this.grammar.Symbols);
            writer.WriteLine($"symbols: {symbols}");

            var terminalSymbols = string.Join(", ", this.grammar.Terminals);
            writer.WriteLine($"terminal-symbols: {terminalSymbols}");

            var nonterminalSymbols = string.Join(", ", this.grammar.Nonterminals);
            writer.WriteLine($"nonterminal-symbols: {nonterminalSymbols}");

            var nullableSymbols = string.Join(", ", this.grammar.Symbols.Where(symbol => symbol.IsNullable));
            writer.WriteLine($"nullable-symbols: {nullableSymbols}");

            writer.WriteLine("----");

            foreach (var production in this.grammar.Productions)
            {
                writer.WriteLine($"{production}");
            }

            writer.WriteLine("----");

            foreach (var rule in this.grammar.Nonterminals)
            {
                Dump(writer, rule);
            }
        }

        private void Dump(TextWriter writer, Nonterminal rule)
        {
            writer.WriteLine($"{rule} =");
            var body = string.Join(" |\n    ", rule.Body.Select(prod => prod.Count == 0 ? "ε" : string.Join(" ", prod)));
            writer.WriteLine($"    {body}");
        }

        private readonly Grammar grammar;
    }
}