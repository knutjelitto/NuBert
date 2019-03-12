using Pliant.Automata;
using Pliant.Grammars;
using Pliant.LexerRules;
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

        private static readonly string root = "Regex";
        public static readonly QualifiedName Regex = new QualifiedName(root, nameof(Regex));
        public static readonly QualifiedName Expression = new QualifiedName(root, nameof(Expression));
        public static readonly QualifiedName Term = new QualifiedName(root, nameof(Term));
        public static readonly QualifiedName Factor = new QualifiedName(root, nameof(Factor));
        public static readonly QualifiedName Atom = new QualifiedName(root, nameof(Atom));
        public static readonly QualifiedName Iterator = new QualifiedName(root, nameof(Iterator));
        public static readonly QualifiedName Set = new QualifiedName(root, nameof(Set));
        public static readonly QualifiedName PositiveSet = new QualifiedName(root, nameof(PositiveSet));
        public static readonly QualifiedName NegativeSet = new QualifiedName(root, nameof(NegativeSet));
        public static readonly QualifiedName CharacterClass = new QualifiedName(root, nameof(CharacterClass));
        public static readonly QualifiedName CharacterRange = new QualifiedName(root, nameof(CharacterRange));
        public static readonly QualifiedName Character = new QualifiedName(root, nameof(Character));
        public static readonly QualifiedName CharacterClassCharacter = new QualifiedName(root, nameof(CharacterClassCharacter));
        
        static RegexGrammar()
        {
            var notMeta = CreateNotMetaLexerRule();
            var notCloseBracket = CreateNotCloseBracketLexerRule();
            var escape = CreateEscapeCharacterLexerRule();

            var regex = NonTerminal.From(Regex);
            var expression = NonTerminal.From(Expression);
            var term = NonTerminal.From(Term);
            var factor = NonTerminal.From(Factor);
            var atom = NonTerminal.From(Atom);
            var iterator = NonTerminal.From(Iterator);
            var set = NonTerminal.From(Set);
            var positiveSet = NonTerminal.From(PositiveSet);
            var negativeSet = NonTerminal.From(NegativeSet);
            var characterClass = NonTerminal.From(CharacterClass);
            var characterRange = NonTerminal.From(CharacterRange);
            var character = NonTerminal.From(Character);
            var characterClassCharacter = NonTerminal.From(CharacterClassCharacter);

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
                Production.From(regex, expression),
                Production.From(regex, caret, expression),
                Production.From(regex, expression, dollar),
                Production.From(regex, caret, expression, dollar),
                Production.From(expression, term),
                Production.From(expression, term, pipe, expression),
                Production.From(term, factor),
                Production.From(term, factor, term),
                Production.From(factor, atom),
                Production.From(factor, atom, iterator),
                Production.From(atom, dot),
                Production.From(atom, character),
                Production.From(atom, openParen, expression, closeParen),
                Production.From(atom, set),
                Production.From(iterator, star),
                Production.From(iterator, plus),
                Production.From(iterator, question),
                Production.From(set, positiveSet),
                Production.From(set, negativeSet),
                Production.From(negativeSet, openBracket, caret, characterClass, closeBracket),
                Production.From(positiveSet, openBracket, characterClass, closeBracket),                
                Production.From(characterClass, characterRange),
                Production.From(characterClass, characterRange, characterClass),
                Production.From(characterRange, characterClassCharacter),
                Production.From(characterRange, characterClassCharacter, minus, characterClassCharacter),
                Production.From(character, notMeta),
                Production.From(character, escape),
                Production.From(characterClassCharacter, notCloseBracket),
                Production.From(characterClassCharacter, escape)
            };

            grammar = new ConcreteGrammar(regex, productions, null, null);
        }
        
        private static LexerRule CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(new SetTerminal(".^$()[]+*?\\/")),
                "NotMeta");
        }

        private static LexerRule CreateNotCloseBracketLexerRule()
        {
            return new TerminalLexerRule(new NegationTerminal(new CharacterTerminal(']')), "NotCloseBracket");
        }

        private static LexerRule CreateEscapeCharacterLexerRule()
        {
            var start = DfaState.Inner();
            var escape = DfaState.Inner();
            var final = DfaState.Final();
            start.AddTransition(new CharacterTerminal('\\'), escape);
            escape.AddTransition(AnyTerminal.Instance, final);
            return new DfaLexerRule(start, "escape");
        }

        public RegexGrammar() : base(grammar)
        {
        }
    }
}