using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pliant.Inputs;
using Pliant.Tokens;

namespace Pliant.Json
{
    public class JsonLexer
    {
        public IEnumerable<IToken> Lex(string input)
        {
            foreach (var token in LexIntern(input))
            {
                yield return token;
            }
        }

        public IEnumerable<IToken> LexIntern(string text)
        {
            var input = new Reader(text);

            var position = 0;
            var builder = new StringBuilder();

            while (true)
            {
                if (!input.Valid)
                {
                    yield break;
                }

                var c = input.Slurp();
                if (char.IsDigit(c)
                    || '+' == c
                    || '-' == c)
                {
                    do
                    {
                        builder.Append(c);
                    }
                    while (input.Accept(char.IsDigit, ref c));

                    if (input.Accept('.'))
                    {
                        builder.Append('.');
                        while (input.Accept(char.IsDigit, ref c))
                        {
                            builder.Append(c);
                        }
                    }

                    yield return new VerbatimToken(Number, position, builder.ToString());

                    position += builder.Length;
                    builder.Clear();
                    continue;
                }

                switch (c)
                {
                    case '[':
                        yield return new VerbatimToken(OpenBracket, position, OpenBracket.Id);
                        break;

                    case ']':
                        yield return new VerbatimToken(CloseBracket, position, CloseBracket.Id);
                        break;

                    case '{':
                        yield return new VerbatimToken(OpenBrace, position, OpenBrace.Id);
                        break;

                    case '}':
                        yield return new VerbatimToken(CloseBrace, position, CloseBrace.Id);
                        break;

                    case ',':
                        yield return new VerbatimToken(Comma, position, Comma.Id);
                        break;

                    case ':':
                        yield return new VerbatimToken(Colon, position, Colon.Id);
                        break;

                    case '"':
                        builder.Append(c);
                        while (input.Accept(x => x != '"', ref c))
                        {
                            builder.Append(c);
                        }

                        if (!input.Accept('"'))
                        {
                            yield break;
                        }

                        builder.Append('"');
                        yield return new VerbatimToken(String, position, builder.ToString());

                        position += builder.Length;
                        builder.Clear();
                        break;

                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        builder.Append(c);
                        while (input.Accept(char.IsWhiteSpace, ref c))
                        {
                            builder.Append(c);
                        }
                        yield return new VerbatimToken(Whitespace, position, builder.ToString());

                        position += builder.Length;
                        builder.Clear();
                        break;

                    case 'n':
                        if (!input.Accept('u'))
                        {
                            yield break;
                        }
                        if (!input.Accept('l'))
                        {
                            yield break;
                        }
                        if (!input.Accept('l'))
                        {
                            yield break;
                        }
                        yield return new VerbatimToken(Null, position, Null.Id);

                        position += 3;
                        break;

                    case 't':
                        if (!input.Accept('r'))
                        {
                            yield break;
                        }
                        if (!input.Accept('u'))
                        {
                            yield break;
                        }
                        if (!input.Accept('e'))
                        {
                            yield break;
                        }
                        yield return new VerbatimToken(True, position, True.Id);

                        position += 3;
                        break;

                    case 'f':
                        if (!input.Accept('a'))
                        {
                            yield break;
                        }
                        if (!input.Accept('l'))
                        {
                            yield break;
                        }
                        if (!input.Accept('s'))
                        {
                            yield break;
                        }
                        if (!input.Accept('e'))
                        {
                            yield break;
                        }

                        yield return new VerbatimToken(False, position, False.Id);

                        position += 4;
                        break;
                }

                position++;
            }
        }


        public static readonly TokenName CloseBrace = new TokenName("}");
        public static readonly TokenName CloseBracket = new TokenName("]");
        public static readonly TokenName Colon = new TokenName(":");
        public static readonly TokenName Comma = new TokenName(",");
        public static readonly TokenName Error = new TokenName("error");
        public static readonly TokenName False = new TokenName("false");
        public static readonly TokenName Null = new TokenName("null");
        public static readonly TokenName Number = new TokenName(@"[-+]?[0-9]*[.]?[0-9]+");
        public static readonly TokenName OpenBrace = new TokenName("{");
        public static readonly TokenName OpenBracket = new TokenName("[");
        public static readonly TokenName String = new TokenName("[\"][^\"]+[\"]");
        public static readonly TokenName True = new TokenName("true");
        public static readonly TokenName Whitespace = new TokenName(@"[\s]+");

        private static bool Accept(TextReader textReader, char c)
        {
            var i = textReader.Peek();
            if (i == -1)
            {
                return false;
            }

            var isAccepted = (char) i == c;
            if (isAccepted)
            {
                textReader.Read();
            }

            return isAccepted;
        }

        private static bool Accept(TextReader textReader, Func<char, bool> predicate, ref char c)
        {
            if (predicate == null)
            {
                return false;
            }

            var i = textReader.Peek();
            if (i == -1)
            {
                return false;
            }

            var isAccepted = predicate((char) i);
            if (!isAccepted)
            {
                return false;
            }

            textReader.Read();
            c = (char) i;
            return true;
        }
    }
}