﻿using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.RegularExpressions
{
    public class RegexGrammar : GrammarWrapper
    {
        private readonly static IGrammar _regexGrammar;

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

        public static readonly string Namespace = nameof(RegularExpressions);
        public static readonly QualifiedName Regex = new QualifiedName(Namespace, nameof(Regex));
        public static readonly QualifiedName Expression = new QualifiedName(Namespace, nameof(Expression));
        public static readonly QualifiedName Term = new QualifiedName(Namespace, nameof(Term));
        public static readonly QualifiedName Factor = new QualifiedName(Namespace, nameof(Factor));
        public static readonly QualifiedName Atom = new QualifiedName(Namespace, nameof(Atom));
        public static readonly QualifiedName Iterator = new QualifiedName(Namespace, nameof(Iterator));
        public static readonly QualifiedName Set = new QualifiedName(Namespace, nameof(Set));
        public static readonly QualifiedName PositiveSet = new QualifiedName(Namespace, nameof(PositiveSet));
        public static readonly QualifiedName NegativeSet = new QualifiedName(Namespace, nameof(NegativeSet));
        public static readonly QualifiedName CharacterClass = new QualifiedName(Namespace, nameof(CharacterClass));
        public static readonly QualifiedName CharacterRange = new QualifiedName(Namespace, nameof(CharacterRange));
        public static readonly QualifiedName Character = new QualifiedName(Namespace, nameof(Character));
        public static readonly QualifiedName CharacterClassCharacter = new QualifiedName(Namespace, nameof(CharacterClassCharacter));
        
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

            _regexGrammar = new Grammar(regex, productions, null, null);
        }
        
        private static LexerRule CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(
                       new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\', '/')),
                "NotMeta");
        }

        private static LexerRule CreateNotCloseBracketLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(
                    new CharacterTerminal(']')),
                "NotCloseBracket");
        }

        private static LexerRule CreateEscapeCharacterLexerRule()
        {
            var start = new DfaState();
            var escape = new DfaState();
            var final = new DfaState(true);
            start.AddTransition(new DfaTransition(new CharacterTerminal('\\'), escape));
            escape.AddTransition(new DfaTransition(new AnyTerminal(), final));
            return new DfaLexerRule(start, "escape");
        }

        public RegexGrammar()
            : base(_regexGrammar)
        {
        }
    }
}