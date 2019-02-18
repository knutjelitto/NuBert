﻿using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Automata;
using Pliant.Tokens;
using Pliant.Grammars;

namespace Pliant.Runtime
{
    public class ParseRunner : IParseRunner
    {
        private readonly TextReader _reader;
        private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
        private readonly List<ILexeme> _tokenLexemes;
        private readonly List<ILexeme> _ignoreLexemes;
        private List<ILexeme> _previousTokenLexemes;
        private readonly List<ILexeme> _triviaAccumulator;
        private readonly List<ILexeme> _triviaLexemes;

        public int Position { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public IParseEngine ParseEngine { get; private set; }

        public ParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseRunner(IParseEngine parseEngine, TextReader reader)
        {
            ParseEngine = parseEngine;
            this._reader = reader;
            this._tokenLexemes = new List<ILexeme>();
            this._ignoreLexemes = new List<ILexeme>();
            this._triviaLexemes = new List<ILexeme>();
            this._triviaAccumulator = new List<ILexeme>();
            this._lexemeFactoryRegistry = new LexemeFactoryRegistry();
            RegisterDefaultLexemeFactories(this._lexemeFactoryRegistry);
            Position = 0;
        }

        public bool Read()
        {
            if (EndOfStream())
            {
                return false;
            }

            var character = ReadCharacter();
            UpdatePositionMetrics(character);

            if (MatchesExistingIncompleteIgnoreLexemes(character))
            {
                return true;
            }

            if (MatchesExistingIncompleteTriviaLexemes(character))
            {
                return true;
            }

            if (MatchExistingTokenLexemes(character))
            {
                if (EndOfStream())
                {
                    return TryParseExistingToken();
                }

                return true;
            }

            if (AnyExistingTokenLexemes())
            {
                if (!TryParseExistingToken())
                {
                    return false;
                }
            }

            if (MatchesNewTokenLexemes(character))
            {
                if (!EndOfStream())
                {
                    if (AnyExistingTriviaLexemes())
                    {
                        AccumulateAcceptedTrivia();
                    }

                    return true;
                }
                return TryParseExistingToken();
            }

            if (MatchesExistingTriviaLexemes(character))
            {
                if (EndOfStream() || IsEndOfLineCharacter(character))
                {
                    AccumulateAcceptedTrivia();
                    AddTrailingTriviaToPreviousToken();
                }
                return true;
            }

            if (AnyExistingTriviaLexemes())
            {
                AccumulateAcceptedTrivia();
            }

            if (MatchesExistingIgnoreLexemes(character))
            {
                return true;
            }

            ClearExistingIgnoreLexemes();

            if (MatchesNewTriviaLexemes(character))
            {
                if (IsEndOfLineCharacter(character))
                {
                    AccumulateAcceptedTrivia();
                    AddTrailingTriviaToPreviousToken();
                }
                return true;
            }

            return MatchesNewIgnoreLexemes(character);
        }

        private bool AnyExistingTriviaLexemes()
        {
            return this._triviaLexemes.Count > 0;
        }

        private void AddTrailingTriviaToPreviousToken()
        {
            if (this._previousTokenLexemes == null
                || this._previousTokenLexemes.Count == 0)
            {
                return;
            }

            for (var a = 0; a < this._triviaAccumulator.Count; a++)
            {
                for (var l = 0; l < this._previousTokenLexemes.Count; l++)
                {
                    this._previousTokenLexemes[l].AddTrailingTrivia(this._triviaAccumulator[a]);
                }
            }

            this._triviaLexemes.Clear();
            this._triviaAccumulator.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePositionMetrics(char character)
        {
            Position++;
            if (IsEndOfLineCharacter(character))
            {
                Column = 0;
                Line++;
            }
            else
            {
                Column++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEndOfLineCharacter(char character)
        {
            switch (character)
            {
                case '\n':
                    return true;
                default:
                    return false;
            }
        }

        public bool EndOfStream()
        {
            return this._reader.Peek() == -1;
        }

        public bool RunToEnd()
        {
            while (!EndOfStream())
            {
                if (!Read())
                {
                    return false;
                }
            }

            return ParseEngine.IsAccepted();
        }

        private char ReadCharacter()
        {
            var character = (char) this._reader.Read();
            return character;
        }

        private bool MatchesNewTriviaLexemes(char character)
        {
            return MatchLexerRules(character, ParseEngine.Grammar.Trivia, this._triviaLexemes);
        }

        private bool MatchesExistingIncompleteTriviaLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, this._triviaLexemes);
        }

        private bool MatchesExistingTriviaLexemes(char character)
        {
            return MatchesExistingLexemes(character, this._triviaLexemes);
        }

        private void AccumulateAcceptedTrivia()
        {
            for (var i = 0; i < this._triviaLexemes.Count; i++)
            {
                var trivia = this._triviaLexemes[i];
                if (trivia.IsAccepted())
                {
                    this._triviaAccumulator.Add(trivia);
                }
            }

            this._triviaLexemes.Clear();
        }

        private bool MatchesExistingIncompleteIgnoreLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, this._ignoreLexemes);
        }

        private bool MatchExistingTokenLexemes(char character)
        {
            return MatchesExistingLexemes(character, this._tokenLexemes);
        }

        private bool TryParseExistingToken()
        {
            var anyLexemes = this._tokenLexemes.Count > 0;
            if (!anyLexemes)
            {
                return false;
            }

            var i = 0;
            var size = this._tokenLexemes.Count;

            while (i < size)
            {
                var lexeme = this._tokenLexemes[i];
                if (lexeme.IsAccepted())
                {
                    i++;
                }
                else
                {
                    if (i < size - 1)
                    {
                        this._tokenLexemes[i] = this._tokenLexemes[size - 1];
                        this._tokenLexemes[size - 1] = lexeme;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
            {
                return false;
            }

            i = this._tokenLexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(this._tokenLexemes[i]);
                this._tokenLexemes.RemoveAt(i);
                i--;
            }

            if (!ParseEngine.Pulse(this._tokenLexemes))
            {
                return false;
            }

            for (i = 0; i < this._triviaAccumulator.Count; i++)
            {
                for (var j = 0; j < this._tokenLexemes.Count; j++)
                {
                    this._tokenLexemes[j].AddLeadingTrivia(this._triviaAccumulator[i]);
                }
            }

            this._triviaAccumulator.Clear();
            if (this._previousTokenLexemes != null)
            {
                this._previousTokenLexemes.Clear();
                this._previousTokenLexemes.AddRange(this._tokenLexemes);
            }
            else
            {
                this._previousTokenLexemes = new List<ILexeme>(this._tokenLexemes);
            }

            this._tokenLexemes.Clear();

            return true;
        }

        private bool MatchesNewTokenLexemes(char character)
        {
            return MatchLexerRules(character, ParseEngine.GetExpectedLexerRules(), this._tokenLexemes);
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            return MatchesExistingLexemes(character, this._ignoreLexemes);
        }

        private void ClearExistingIgnoreLexemes()
        {
            ClearLexemes(this._ignoreLexemes);
        }

        private bool MatchesNewIgnoreLexemes(char character)
        {
            return MatchLexerRules(character, ParseEngine.Grammar.Ignores, this._ignoreLexemes);
        }

        private bool MatchLexerRules(char character, IReadOnlyList<ILexerRule> lexerRules, List<ILexeme> lexemes)
        {
            var anyMatches = false;
            for (var i = 0; i < lexerRules.Count; i++)
            {
                var lexerRule = lexerRules[i];
                if (!lexerRule.CanApply(character))
                {
                    continue;
                }

                var factory = this._lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
                var lexeme = factory.Create(lexerRule, Position);

                if (!lexeme.Scan(character))
                {
                    FreeLexeme(lexeme);
                    continue;
                }

                if (!anyMatches)
                {
                    anyMatches = true;
                    lexemes.Clear();
                }

                lexemes.Add(lexeme);
            }

            return anyMatches;
        }

        private bool MatchesExistingLexemes(char character, List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
            {
                return false;
            }

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (lexeme.Scan(character))
                {
                    i++;
                }
                else
                {
                    if (i < size - 1)
                    {
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = lexeme;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
            {
                return false;
            }

            i = lexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(lexemes[i]);
                lexemes.RemoveAt(i);
                i--;
            }
            return true;
        }

        private bool MatchesExistingIncompleteLexemes(char character, List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
            {
                return false;
            }

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (!lexeme.IsAccepted() && lexeme.Scan(character))
                {
                    i++;
                }
                else
                {
                    if (i < size - 1)
                    {
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = lexeme;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
            {
                return false;
            }

            i = lexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(lexemes[i]);
                lexemes.RemoveAt(i);
                i--;
            }
            return true;
        }

        private void ClearLexemes(List<ILexeme> lexemes)
        {
            for (var i = 0; i < lexemes.Count; i++)
            {
                FreeLexeme(lexemes[i]);
            }

            lexemes.Clear();
        }

        private void FreeLexeme(ILexeme lexeme)
        {
            var lexemeFactory = this._lexemeFactoryRegistry.Get(lexeme.LexerRule.LexerRuleType);
            lexemeFactory.Free(lexeme);
        }

        private bool AnyExistingTokenLexemes()
        {
            return this._tokenLexemes.Count > 0;
        }

        private static void RegisterDefaultLexemeFactories(ILexemeFactoryRegistry lexemeFactoryRegistry)
        {
            lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
            lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
            lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
            lexemeFactoryRegistry.Register(new DfaLexemeFactory());
        }

    }
}