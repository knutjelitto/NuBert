using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;

namespace Pliant.Json
{
    public class JsonGrammar : GrammarWrapper
    {
        static JsonGrammar()
        {
            ProductionExpression
                Json = "Json",
                Object = "Object",
                Pair = "Pair",
                PairRepeat = "PairRepeat",
                Array = "Array",
                Value = "Value",
                ValueRepeat = "ValueRepeat";

            var number = new NumberLexerRule();
            var @string = String();

            Json.Rule =
                Value;

            Object.Rule =
                '{' + PairRepeat + '}';

            PairRepeat.Rule =
                Pair
                | (Pair + ',' + PairRepeat)
                | Expr.Epsilon;

            Pair.Rule =
                (Expr) @string + ':' + Value;

            Array.Rule =
                '[' + ValueRepeat + ']';

            ValueRepeat.Rule =
                Value
                | (Value + ',' + ValueRepeat)
                | Expr.Epsilon;

            Value.Rule =
                (Expr) @string
                | number
                | Object
                | Array
                | "true"
                | "false"
                | "null";

            grammar = new GrammarExpression(
                    Json,
                    null,
                    new[] {new WhitespaceLexerRule()})
                .ToGrammar();
        }

        public JsonGrammar() : base(grammar)
        {
        }

        private static LexerRule CreateRegexDfa(string pattern)
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse(pattern);
            var regexCompiler = new RegexCompiler();
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexerRule(dfa, pattern);
        }

        private static LexerRule String()
        {
            // ["][^"]+["]
            const string pattern = "[\"][^\"]+[\"]";
            return CreateRegexDfa(pattern);
        }

        private static readonly Grammar grammar;
    }
}