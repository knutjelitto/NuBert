using Pliant.Automata;
using Pliant.Builders;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Tokens;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : GrammarWrapper
    {
        static EbnfGrammar()
        {
            LexerRule
                settingIdentifier = CreateSettingIdentifierLexerRule(),
                identifier = CreateIdentifierLexerRule(),
                any = new TerminalLexerRule(new AnyTerminal(), "."),
                notCloseBracket = new TerminalLexerRule(new NegationTerminal(new CharacterTerminal(']')), "[^\\]]"),
                escapeCharacter = CreateEscapeCharacterLexerRule(),
                whitespace = CreateWhitespaceLexerRule(),
                multiLineComment = CreateMultiLineCommentLexerRule();

            ProductionExpression
                definition = Definition,
                block = Block,
                rule = Rule,
                setting = Setting,
                lexerRule = LexerRule,
                qualifiedIdentifier = QualifiedIdentifier,
                expression = Expression,
                term = Term,
                factor = Factor,
                literal = Literal,
                grouping = Grouping,
                repetition = Repetition,
                optional = Optional,
                lexerRuleExpression = LexerRuleExpression,
                lexerRuleTerm = LexerRuleTerm,
                lexerRuleFactor = LexerRuleFactor;

            var regexGrammar = new RegexGrammar();
            var regexProductionReference = new ProductionReferenceExpression(regexGrammar);

            definition.Rule =
                block
                | (block + definition);

            block.Rule =
                rule
                | setting
                | lexerRule;

            rule.Rule =
                qualifiedIdentifier + '=' + expression + ';';

            setting.Rule = (Expr)
                           settingIdentifier + '=' + qualifiedIdentifier + ';';

            lexerRule.Rule =
                qualifiedIdentifier + '~' + lexerRuleExpression + ';';

            expression.Rule =
                term
                | (term + '|' + expression);

            term.Rule =
                factor
                | (factor + term);

            factor.Rule
                = qualifiedIdentifier
                  | literal
                  | ('/' + regexProductionReference + '/')
                  | repetition
                  | optional
                  | grouping;

            literal.Rule = (Expr)
                           new SimpleSingleQuoteStringLexerRule()
                           | new SimpleDoubleQuoteStringLexerRule();

            repetition.Rule = (Expr)
                              '{' + expression + '}';

            optional.Rule = (Expr)
                            '[' + expression + ']';

            grouping.Rule = (Expr)
                            '(' + expression + ')';

            qualifiedIdentifier.Rule =
                identifier
                | ((Expr) identifier + '.' + qualifiedIdentifier);

            lexerRuleExpression.Rule =
                lexerRuleTerm
                | (lexerRuleTerm + '|' + lexerRuleExpression);

            lexerRuleTerm.Rule =
                lexerRuleFactor
                | (lexerRuleFactor + lexerRuleTerm);

            lexerRuleFactor.Rule =
                literal
                | ('/' + regexProductionReference + '/');

            var grammarExpression = new GrammarExpression(
                definition,
                new[]
                {
                    definition,
                    block,
                    rule,
                    setting,
                    lexerRule,
                    expression,
                    term,
                    factor,
                    literal,
                    repetition,
                    optional,
                    grouping,
                    qualifiedIdentifier,
                    lexerRuleExpression,
                    lexerRuleTerm,
                    lexerRuleFactor
                },
                new[] { whitespace, multiLineComment });
            ebnfGrammar = grammarExpression.ToGrammar();
        }

        public EbnfGrammar()
            : base(ebnfGrammar)
        {
        }

        private static readonly string Namespace = "Ebnf";
        public static readonly QualifiedName Block = new QualifiedName(Namespace, nameof(Block));
        public static readonly QualifiedName Definition = new QualifiedName(Namespace, nameof(Definition));
        public static readonly QualifiedName Expression = new QualifiedName(Namespace, nameof(Expression));
        public static readonly QualifiedName Factor = new QualifiedName(Namespace, nameof(Factor));
        public static readonly QualifiedName Grouping = new QualifiedName(Namespace, nameof(Grouping));
        public static readonly QualifiedName LexerRule = new QualifiedName(Namespace, nameof(LexerRule));
        public static readonly QualifiedName LexerRuleExpression = new QualifiedName(Namespace, nameof(LexerRuleExpression));
        public static readonly QualifiedName LexerRuleFactor = new QualifiedName(Namespace, nameof(LexerRuleFactor));
        public static readonly QualifiedName LexerRuleTerm = new QualifiedName(Namespace, nameof(LexerRuleTerm));
        public static readonly QualifiedName Literal = new QualifiedName(Namespace, nameof(Literal));

        public static readonly QualifiedName Optional = new QualifiedName(Namespace, nameof(Optional));
        public static readonly QualifiedName QualifiedIdentifier = new QualifiedName(Namespace, nameof(QualifiedIdentifier));
        public static readonly QualifiedName Repetition = new QualifiedName(Namespace, nameof(Repetition));
        public static readonly QualifiedName Rule = new QualifiedName(Namespace, nameof(Rule));
        public static readonly QualifiedName Setting = new QualifiedName(Namespace, nameof(Setting));
        public static readonly QualifiedName Term = new QualifiedName(Namespace, nameof(Term));

        private static LexerRule CreateEscapeCharacterLexerRule()
        {
            var start = DfaState.Inner();
            var escape = DfaState.Inner();
            var final = DfaState.Final();
            start.AddTransition(new DfaTransition(new CharacterTerminal('\\'), escape));
            escape.AddTransition(new DfaTransition(new AnyTerminal(), final));
            return new DfaLexerRule(start, TokenTypes.Escape);
        }

        private static LexerRule CreateIdentifierLexerRule()
        {
            // /[a-zA-Z][a-zA-Z0-9-_]*/
            var identifierState = DfaState.Inner();
            var zeroOrMoreLetterOrDigit = DfaState.Final();
            identifierState.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z')),
                    zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z'),
                        new DigitTerminal(),
                        new SetTerminal('-', '_')),
                    zeroOrMoreLetterOrDigit));
            var identifier = new DfaLexerRule(identifierState, TokenTypes.Identifier);
            return identifier;
        }

        private static LexerRule CreateMultiLineCommentLexerRule()
        {
            var states = new DfaState[5];
            for (var i = 0; i < states.Length; i++)
            {
                states[i] = DfaState.Inner();
            }

            var slash = new CharacterTerminal('/');
            var star = new CharacterTerminal('*');
            var notStar = new NegationTerminal(star);
            var notSlash = new NegationTerminal(slash);

            var firstSlash = new DfaTransition(slash, states[1]);
            var firstStar = new DfaTransition(star, states[2]);
            var repeatNotStar = new DfaTransition(notStar, states[2]);
            var lastStar = new DfaTransition(star, states[3]);
            var goBackNotSlash = new DfaTransition(notSlash, states[2]);
            var lastSlash = new DfaTransition(slash, states[4]);

            states[0].AddTransition(firstSlash);
            states[1].AddTransition(firstStar);
            states[2].AddTransition(repeatNotStar);
            states[2].AddTransition(lastStar);
            states[3].AddTransition(goBackNotSlash);
            states[3].AddTransition(lastSlash);

            return new DfaLexerRule(states[0], TokenTypes.MultiLineComment);
        }

        private static LexerRule CreateNotDoubleQuoteLexerRule()
        {
            // ([^"]|(\\.))*
            var start = DfaState.Inner();
            var escape = DfaState.Inner();
            var final = DfaState.Final();

            var notDoubleQuoteTerminal = new NegationTerminal(
                new CharacterTerminal('"'));
            var escapeTerminal = new CharacterTerminal('\\');
            var anyTerminal = new AnyTerminal();

            var notDoubleQuoteEdge = new DfaTransition(notDoubleQuoteTerminal, final);
            start.AddTransition(notDoubleQuoteEdge);
            final.AddTransition(notDoubleQuoteEdge);

            var escapeEdge = new DfaTransition(escapeTerminal, escape);
            start.AddTransition(escapeEdge);
            final.AddTransition(escapeEdge);

            var anyEdge = new DfaTransition(anyTerminal, final);
            escape.AddTransition(anyEdge);

            return new DfaLexerRule(start, TokenTypes.NotDoubleQuote);
        }

        private static LexerRule CreateNotSingleQuoteLexerRule()
        {
            var start = DfaState.Inner();
            var final = DfaState.Final();
            var terminal = new NegationTerminal(new CharacterTerminal('\''));
            var edge = new DfaTransition(terminal, final);
            start.AddTransition(edge);
            final.AddTransition(edge);
            return new DfaLexerRule(start, TokenTypes.NotSingleQuote);
        }

        private static LexerRule CreateSettingIdentifierLexerRule()
        {
            // /:[a-zA-Z][a-zA-Z0-9]*/
            var start = DfaState.Inner();
            var oneLetter = DfaState.Inner();
            var zeroOrMoreLetterOrDigit = DfaState.Final();
            start.AddTransition(
                new DfaTransition(
                    new CharacterTerminal(':'),
                    oneLetter));
            oneLetter.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z')),
                    zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z'),
                        new DigitTerminal()),
                    zeroOrMoreLetterOrDigit));
            return new DfaLexerRule(start, TokenTypes.SettingIdentifier);
        }

        private static LexerRule CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = DfaState.Inner();
            var finalWhitespace = DfaState.Final();
            startWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, TokenTypes.Whitespace);
            return whitespace;
        }

        private static readonly IGrammar ebnfGrammar;
        public static class TokenTypes
        {
            public static readonly TokenType Escape = new TokenType("escape");
            public static readonly TokenType Identifier = new TokenType("EbnfQualifiedIdentifier");
            public static readonly TokenType MultiLineComment = new TokenType(@"\/[*]([*][^\/]|[^*])*[*][\/]");
            public static readonly TokenType NotDoubleQuote = new TokenType(@"([^""]|(\\.))+");
            public static readonly TokenType NotSingleQuote = new TokenType(@"([^']|(\\.))+");
            public static readonly TokenType SettingIdentifier = new TokenType("settingIdentifier");
            public static readonly TokenType Whitespace = new TokenType("whitespace");
        }
    }
}