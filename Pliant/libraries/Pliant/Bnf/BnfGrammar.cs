using Pliant.Automata;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Bnf
{
    public class BnfGrammar : GrammarWrapper
    {
        static BnfGrammar()
        {
            /*
             *  <grammar>        ::= <rule> | <rule> <grammar>
             *  <rule>           ::= "<" <rule-name> ">" "::=" <expression>
             *  <expression>     ::= <list> | <list> "|" <expression>
             *  <line-end>       ::= <EOL> | <line-end> <line-end>
             *  <list>           ::= <term> | <term> <list>
             *  <term>           ::= <literal> | "<" <rule-name> ">"
             *  <literal>        ::= '"' <text> '"' | "'" <text> "'"
             */
            var whitespace = CreateWhitespaceLexerRule();
            var ruleName = CreateRuleNameLexerRule();
            var implements = CreateImplementsLexerRule();
            var eol = CreateEndOfLineLexerRule();
            var notDoubleQuote = CreateNotDoubleQuoteLexerRule();
            var notSingleQuote = CreateNotSingleQuoteLexerRule();

            var grammar = new NonTerminal("grammar");
            var rule = new NonTerminal("rule");
            var identifier = new NonTerminal("EbnfQualifiedIdentifier");
            var expression = new NonTerminal("expression");
            var lineEnd = new NonTerminal("line-end");
            var list = new NonTerminal("list");
            var term = new NonTerminal("term");
            var literal = new NonTerminal("literal");

            var lessThan = new TerminalLexerRule('<');
            var greaterThan = new TerminalLexerRule('>');
            var doubleQuote = new TerminalLexerRule('"');
            var slash = new TerminalLexerRule('\'');
            var pipe = new TerminalLexerRule('|');

            var productions = new[]
            {
                new Production(grammar, rule),
                new Production(grammar, rule, grammar),
                new Production(rule, identifier, implements, expression),
                new Production(expression, list),
                new Production(expression, list, pipe, expression),
                new Production(lineEnd, eol),
                new Production(lineEnd, lineEnd, lineEnd),
                new Production(list, term),
                new Production(list, term, list),
                new Production(term, literal),
                new Production(term, identifier),
                new Production(identifier, lessThan, ruleName, greaterThan),
                new Production(literal, doubleQuote, notDoubleQuote, doubleQuote),
                new Production(literal, slash, notSingleQuote, slash)
            };

            var ignore = new[]
            {
                whitespace
            };

            _bnfGrammar = new ConcreteGrammar(grammar, productions, ignore, null);
        }

        public BnfGrammar()
            : base(_bnfGrammar)
        {
        }

        private static Lexer CreateEndOfLineLexerRule()
        {
            return new StringLiteralLexer("\r\n", new TokenType("eol"));
        }

        private static Lexer CreateImplementsLexerRule()
        {
            return new StringLiteralLexer("::=", new TokenType("implements"));
        }

        private static Lexer CreateNotDoubleQuoteLexerRule()
        {
            // ( [^"\\] | (\\ .) ) +
            var start = DfaState.Inner();
            var escape = DfaState.Inner();
            var final = DfaState.Final();

            var notQuoteTerminal = new NegationTerminal(new SetTerminal('"', '\\'));
            var escapeTerminal = new CharacterTerminal('\\');

            start.AddTransition(notQuoteTerminal, final);
            final.AddTransition(notQuoteTerminal, final);

            start.AddTransition(escapeTerminal, escape);
            final.AddTransition(escapeTerminal, escape);

            escape.AddTransition(AnyTerminal.Instance, final);

            return new DfaLexer(start, "not-double-quote");
        }

        private static Lexer CreateNotSingleQuoteLexerRule()
        {
            var start = DfaState.Inner();
            var final = DfaState.Final();
            var terminal = new NegationTerminal(new CharacterTerminal('\''));
            start.AddTransition(terminal, final);
            final.AddTransition(terminal, final);
            return new DfaLexer(start, "not-single-quote");
        }

        private static Lexer CreateRuleNameLexerRule()
        {
            // /[a-zA-Z][a-zA-Z0-9-_]*/
            var ruleNameState = DfaState.Inner();
            var zeroOrMoreLetterOrDigit = DfaState.Final();
            ruleNameState.AddTransition(
                AsciiLetterTerminal.Instance,
                zeroOrMoreLetterOrDigit);
            zeroOrMoreLetterOrDigit.AddTransition(
                new CharacterClassTerminal(
                    AsciiLetterTerminal.Instance,
                    DigitTerminal.Instance,
                    new SetTerminal('-', '_')),
                zeroOrMoreLetterOrDigit);
            var ruleName = new DfaLexer(ruleNameState, "rule-name");
            return ruleName;
        }

        private static Lexer CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            var startState = DfaState.Inner();
            var finalState = DfaState.Final();
            startState.AddTransition(whitespaceTerminal, finalState);
            finalState.AddTransition(whitespaceTerminal, finalState);
            return new DfaLexer(startState, "[\\s]+");
        }

        private static readonly Grammar _bnfGrammar;
    }
}