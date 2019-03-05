using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Runtime
{
    public class ParseRunner : IParseRunner
    {
        public ParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseRunner(IParseEngine parseEngine, TextReader reader)
        {
            ParseEngine = parseEngine;
            this.reader = reader;
            this.tokenLexemes = new List<Lexeme>();
            this.ignoreLexemes = new List<Lexeme>();
            this.triviaLexemes = new List<Lexeme>();
            this.triviaAccumulator = new List<Lexeme>();
            this.previousTokenLexemes = new List<Lexeme>();
            Position = 0;
        }

        public int Column { get; private set; }

        public int Line { get; private set; }

        public IParseEngine ParseEngine { get; }

        public int Position { get; private set; }

        public bool EndOfStream()
        {
            return this.reader.Peek() == -1;
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

        private void AccumulateAcceptedTrivia()
        {
            foreach (var trivia in this.triviaLexemes)
            {
                if (trivia.IsAccepted())
                {
                    this.triviaAccumulator.Add(trivia);
                }
            }

            this.triviaLexemes.Clear();
        }

        private void AddTrailingTriviaToPreviousToken()
        {
            if (this.previousTokenLexemes.Count == 0)
            {
                return;
            }

            foreach (var trivia in this.triviaAccumulator)
            {
                foreach (var lexeme in this.previousTokenLexemes)
                {
                    lexeme.AddTrailingTrivia(trivia);
                }
            }

            this.triviaLexemes.Clear();
            this.triviaAccumulator.Clear();
        }

        private bool AnyExistingTokenLexemes()
        {
            return this.tokenLexemes.Count > 0;
        }

        private bool AnyExistingTriviaLexemes()
        {
            return this.triviaLexemes.Count > 0;
        }

        private void ClearExistingIgnoreLexemes()
        {
            this.ignoreLexemes.Clear();
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            return MatchesExistingLexemes(character, this.ignoreLexemes);
        }

        private bool MatchesExistingIncompleteIgnoreLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, this.ignoreLexemes);
        }

        private bool MatchesExistingIncompleteLexemes(char character, List<Lexeme> lexemes)
        {
            if (lexemes == null || lexemes.Count == 0)
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
                lexemes.RemoveAt(i);
                i--;
            }

            return true;
        }

        private bool MatchesExistingIncompleteTriviaLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, this.triviaLexemes);
        }

        private bool MatchesExistingLexemes(char character, List<Lexeme> lexemes)
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
                lexemes.RemoveAt(i);
                i--;
            }

            return true;
        }

        private bool MatchesExistingTriviaLexemes(char character)
        {
            return MatchesExistingLexemes(character, this.triviaLexemes);
        }

        private bool MatchesNewIgnoreLexemes(char character)
        {
            return MatchLexers(character, ParseEngine.Grammar.Ignores, this.ignoreLexemes);
        }

        private bool MatchesNewTokenLexemes(char character)
        {
            return MatchLexers(character, ParseEngine.GetExpectedLexerRules(), this.tokenLexemes);
        }

        private bool MatchesNewTriviaLexemes(char character)
        {
            return MatchLexers(character, ParseEngine.Grammar.Trivia, this.triviaLexemes);
        }

        private bool MatchExistingTokenLexemes(char character)
        {
            return MatchesExistingLexemes(character, this.tokenLexemes);
        }

        private bool MatchLexers(char character, IEnumerable<LexerRule> lexers, ICollection<Lexeme> lexemes)
        {
            var anyMatches = false;

            foreach (var lexer in lexers)
            {
                if (!lexer.CanApply(character))
                {
                    continue;
                }

                var lexeme = lexer.CreateLexeme(Position);

                if (!lexeme.Scan(character))
                {
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

        private char ReadCharacter()
        {
            var character = (char) this.reader.Read();
            return character;
        }

        private bool TryParseExistingToken()
        {
            var anyLexemes = this.tokenLexemes.Count > 0;
            if (!anyLexemes)
            {
                return false;
            }

            var i = 0;
            var size = this.tokenLexemes.Count;

            while (i < size)
            {
                var lexeme = this.tokenLexemes[i];
                if (lexeme.IsAccepted())
                {
                    i++;
                }
                else
                {
                    if (i < size - 1)
                    {
                        this.tokenLexemes[i] = this.tokenLexemes[size - 1];
                        this.tokenLexemes[size - 1] = lexeme;
                    }

                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
            {
                return false;
            }

            i = this.tokenLexemes.Count - 1;
            while (i >= size)
            {
                this.tokenLexemes.RemoveAt(i);
                i--;
            }

            if (!ParseEngine.Pulse(this.tokenLexemes))
            {
                return false;
            }

            for (i = 0; i < this.triviaAccumulator.Count; i++)
            {
                foreach (var tokenLexeme in this.tokenLexemes)
                {
                    tokenLexeme.AddLeadingTrivia(this.triviaAccumulator[i]);
                }
            }

            this.triviaAccumulator.Clear();
            this.previousTokenLexemes.Clear();
            this.previousTokenLexemes.AddRange(this.tokenLexemes);

            this.tokenLexemes.Clear();

            return true;
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

        private readonly List<Lexeme> ignoreLexemes;
        private readonly List<Lexeme> previousTokenLexemes;
        private readonly TextReader reader;
        private readonly List<Lexeme> tokenLexemes;
        private readonly List<Lexeme> triviaAccumulator;
        private readonly List<Lexeme> triviaLexemes;
    }
}