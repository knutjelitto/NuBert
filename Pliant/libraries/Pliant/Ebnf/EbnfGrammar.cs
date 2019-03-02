using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Terminals;
using Pliant.Tokens;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : GrammarWrapper
    {
        static EbnfGrammar()
        {
            Lexer
                settingIdentifier = CreateSettingIdentifierLexerRule(),
                identifier = CreateIdentifierLexerRule(),
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

            var regexProductionReference = new GrammarReferenceExpression(new RegexGrammar());

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

        private static Lexer CreateIdentifierLexerRule()
        {
            // /[a-zA-Z][a-zA-Z0-9-_]*/
            var identifierState = DfaState.Inner();
            var zeroOrMoreLetterOrDigit = DfaState.Final();
            identifierState.AddTransition(
                AsciiLetterTerminal.Instance,
                zeroOrMoreLetterOrDigit);
            zeroOrMoreLetterOrDigit.AddTransition(
                new CharacterClassTerminal(
                    AsciiLetterTerminal.Instance,
                    DigitTerminal.Instance,
                    new SetTerminal('-', '_')),
                zeroOrMoreLetterOrDigit);

            return new DfaLexer(identifierState, TokenTypes.Identifier);
        }

        private static Lexer CreateSingleLineComment()
        {
            var slash = new CharacterTerminal('/');
            var newLine = new CharacterTerminal('\n');
            var notNewLine = new NegationTerminal(newLine);

            var start = DfaState.Inner();
            var oneSlash = DfaState.Inner();
            var twoSlash = DfaState.Final();

            start.AddTransition(slash, oneSlash);
            oneSlash.AddTransition(slash, twoSlash);
            twoSlash.AddTransition(notNewLine, twoSlash);

            return new DfaLexer(start, TokenTypes.SingleLineComment);
        }

        private static Lexer CreateMultiLineCommentLexerRule()
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


            states[0].AddTransition(slash, states[1]);
            states[1].AddTransition(star, states[2]);
            states[2].AddTransition(notStar, states[2]);
            states[2].AddTransition(star, states[3]);
            states[3].AddTransition(notSlash, states[2]);
            states[3].AddTransition(slash, states[4]);

            return new DfaLexer(states[0], TokenTypes.MultiLineComment);
        }

        private static Lexer CreateSettingIdentifierLexerRule()
        {
            // /:[a-zA-Z][a-zA-Z0-9]*/
            var start = DfaState.Inner();
            var oneLetter = DfaState.Inner();
            var zeroOrMoreLetterOrDigit = DfaState.Final();
            start.AddTransition(new CharacterTerminal(':'), oneLetter);
            oneLetter.AddTransition(
                AsciiLetterTerminal.Instance,
                zeroOrMoreLetterOrDigit);
            zeroOrMoreLetterOrDigit.AddTransition(
                new CharacterClassTerminal(
                    AsciiLetterTerminal.Instance,
                    DigitTerminal.Instance),
                zeroOrMoreLetterOrDigit);
            return new DfaLexer(start, TokenTypes.SettingIdentifier);
        }

        private static Lexer CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = WhitespaceTerminal.Instance;
            var startWhitespace = DfaState.Inner();
            var finalWhitespace = DfaState.Final();
            startWhitespace.AddTransition(whitespaceTerminal, finalWhitespace);
            finalWhitespace.AddTransition(whitespaceTerminal, finalWhitespace);
            var whitespace = new DfaLexer(startWhitespace, TokenTypes.Whitespace);
            return whitespace;
        }

        private static readonly IGrammar ebnfGrammar;
        public static class TokenTypes
        {
            public static readonly TokenType Identifier = new TokenType("EbnfQualifiedIdentifier");
            public static readonly TokenType MultiLineComment = new TokenType(@"\/[*]([*][^\/]|[^*])*[*][\/]");
            public static readonly TokenType SingleLineComment = new TokenType(@"\/\/.*$");
            public static readonly TokenType SettingIdentifier = new TokenType("settingIdentifier");
            public static readonly TokenType Whitespace = new TokenType("whitespace");
        }
    }
}