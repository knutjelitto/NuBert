using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.RegularExpressions
{
    public class RegexGrammar : GrammarWrapper
    {
        private static readonly Grammar grammar;

        /*  Regex                      ->   Expression |
         *                                  '^' Expression |
         *                                  Expression '$' |
         *                                  '^' Expression '$'
         *
         *  Expresion                  ->   Term |
         *                                  Term '|' Expression
         *                                  λ
         *
         *  Term                       ->   Factor |
         *                                  Factor Term
         *
         *  Factor                     ->   Atom |
         *                                  Atom Iterator
         *
         *  Atom                       ->   . |
         *                                  Character |
         *                                  '(' Expression ')' |
         *                                  Set
         *
         *  Set                        ->   PositiveSet |
         *                                  NegativeSet
         *
         *  PositiveSet                ->   '[' CharacterClass ']'
         *
         *  NegativeSet                ->   "[^" CharacterClass ']'
         *
         *  CharacterClass             ->   CharacterRange |
         *                                  CharacterRange CharacterClass
         *
         *  CharacterRange             ->   CharacterClassCharacter |
         *                                  CharacterClassCharacter '-' CharacterClassCharacter
         *
         *  Character                  ->   NotMetaCharacter |
         *                                  EscapeSequence
         *
         *  CharacterClassCharacter    ->   NotCloseBracketCharacter |
         *                                  EscapeSequence
         */

        private static readonly string name = nameof(RegularExpressions);
        public static readonly QualifiedName Regex = new QualifiedName(name, nameof(Regex));
        public static readonly QualifiedName Expression = new QualifiedName(name, nameof(Expression));
        public static readonly QualifiedName Term = new QualifiedName(name, nameof(Term));
        public static readonly QualifiedName Factor = new QualifiedName(name, nameof(Factor));
        public static readonly QualifiedName Atom = new QualifiedName(name, nameof(Atom));
        public static readonly QualifiedName Iterator = new QualifiedName(name, nameof(Iterator));
        public static readonly QualifiedName Set = new QualifiedName(name, nameof(Set));
        public static readonly QualifiedName PositiveSet = new QualifiedName(name, nameof(PositiveSet));
        public static readonly QualifiedName NegativeSet = new QualifiedName(name, nameof(NegativeSet));
        public static readonly QualifiedName CharacterClass = new QualifiedName(name, nameof(CharacterClass));
        public static readonly QualifiedName CharacterRange = new QualifiedName(name, nameof(CharacterRange));
        public static readonly QualifiedName Character = new QualifiedName(name, nameof(Character));
        public static readonly QualifiedName CharacterClassCharacter = new QualifiedName(name, nameof(CharacterClassCharacter));
        
        static RegexGrammar()
        {
            var notMeta = CreateNotMetaLexerRule();
            var notCloseBracket = CreateNotCloseBracketLexerRule();
            var escape = CreateEscapeCharacterLexerRule();

            var regex = new NonTerminal(Regex);
            var expression = new NonTerminal(Expression);
            var term = new NonTerminal(Term);
            var factor = new NonTerminal(Factor);
            var atom = new NonTerminal(Atom);
            var iterator = new NonTerminal(Iterator);
            var set = new NonTerminal(Set);
            var positiveSet = new NonTerminal(PositiveSet);
            var negativeSet = new NonTerminal(NegativeSet);
            var characterClass = new NonTerminal(CharacterClass);
            var characterRange = new NonTerminal(CharacterRange);
            var character = new NonTerminal(Character);
            var characterClassCharacter = new NonTerminal(CharacterClassCharacter);

            var caret = new TerminalLexerRule('^');
            var dollar = new TerminalLexerRule('$');
            var pipe = new TerminalLexerRule('|');
            var dot = new TerminalLexerRule('.');
            var openParen = new TerminalLexerRule('(');
            var closeParen = new TerminalLexerRule(')');
            var star = new TerminalLexerRule('*');
            var plus = new TerminalLexerRule('+');
            var question = new TerminalLexerRule('?');
            var openBracket = new TerminalLexerRule('[');
            var closeBracket = new TerminalLexerRule(']');
            var minus = new TerminalLexerRule('-');

            var productions = new[]
            {
                new Production(regex, expression),
                new Production(regex, caret, expression),
                new Production(regex, expression, dollar),
                new Production(regex, caret, expression, dollar),
                new Production(expression, term),
                new Production(expression, term, pipe, expression),
                new Production(term, factor),
                new Production(term, factor, term),
                new Production(factor, atom),
                new Production(factor, atom, iterator),
                new Production(atom, dot),
                new Production(atom, character),
                new Production(atom, openParen, expression, closeParen),
                new Production(atom, set),
                new Production(iterator, star),
                new Production(iterator, plus),
                new Production(iterator, question),
                new Production(set, positiveSet),
                new Production(set, negativeSet),
                new Production(negativeSet, openBracket, caret, characterClass, closeBracket),
                new Production(positiveSet, openBracket, characterClass, closeBracket),                
                new Production(characterClass, characterRange),
                new Production(characterClass, characterRange, characterClass),
                new Production(characterRange, characterClassCharacter),
                new Production(characterRange, characterClassCharacter, minus, characterClassCharacter),
                new Production(character, notMeta),
                new Production(character, escape),
                new Production(characterClassCharacter, notCloseBracket),
                new Production(characterClassCharacter, escape)
            };

            grammar = new GrammarImpl(regex, productions, null, null);
        }
        
        private static Lexer CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\', '/')),
                "NotMeta");
        }

        private static Lexer CreateNotCloseBracketLexerRule()
        {
            return new TerminalLexerRule(new NegationTerminal(new CharacterTerminal(']')), "NotCloseBracket");
        }

        private static Lexer CreateEscapeCharacterLexerRule()
        {
            var start = DfaState.Inner();
            var escape = DfaState.Inner();
            var final = DfaState.Final();
            start.AddTransition(new CharacterTerminal('\\'), escape);
            escape.AddTransition(AnyTerminal.Instance, final);
            return new DfaLexer(start, "escape");
        }

        public RegexGrammar() : base(grammar)
        {
        }
    }
}