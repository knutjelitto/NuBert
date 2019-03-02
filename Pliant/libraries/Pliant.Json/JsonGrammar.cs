using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;

namespace Pliant.Json
{
    public class JsonGrammar : GrammarWrapper
    {
        private static readonly IGrammar _grammar;

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
                (Expr)@string + ':' + Value;

            Array.Rule =
                '[' + ValueRepeat + ']';

            ValueRepeat.Rule =
                Value
                | (Value + ',' + ValueRepeat)
                | Expr.Epsilon;

            Value.Rule =
                (Expr)@string
                | number
                | Object
                | Array
                | "true"
                | "false"
                | "null";

            _grammar = new GrammarExpression(
                Json,
                null,
                new[] { new WhitespaceLexerRule() })
            .ToGrammar();
        }

        public JsonGrammar() : base(_grammar)
        {
        }
        
        private static Lexer String()
        {
            // ["][^"]+["]
            const string pattern = "[\"][^\"]+[\"]";
            return CreateRegexDfa(pattern);
        }

        private static Lexer CreateRegexDfa(string pattern)
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse(pattern);
            var regexCompiler = new RegexCompiler();
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexer(dfa, pattern);
        }

    }
}
