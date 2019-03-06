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

                    yield return new VerbatimToken(position, builder.ToString(), Number);

                    position += builder.Length;
                    builder.Clear();
                    continue;
                }

                switch (c)
                {
                    case '[':
                        yield return new VerbatimToken(position, OpenBracket.Id, OpenBracket);
                        break;

                    case ']':
                        yield return new VerbatimToken(position, CloseBracket.Id, CloseBracket);
                        break;

                    case '{':
                        yield return new VerbatimToken(position, OpenBrace.Id, OpenBrace);
                        break;

                    case '}':
                        yield return new VerbatimToken(position, CloseBrace.Id, CloseBrace);
                        break;

                    case ',':
                        yield return new VerbatimToken(position, Comma.Id, Comma);
                        break;

                    case ':':
                        yield return new VerbatimToken(position, Colon.Id, Colon);
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
                        yield return new VerbatimToken(position, builder.ToString(), String);

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
                        yield return new VerbatimToken(position, builder.ToString(), Whitespace);

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
                        yield return new VerbatimToken(position, Null.Id, Null);

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
                        yield return new VerbatimToken(position, True.Id, True);

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

                        yield return new VerbatimToken(position, False.Id, False);

                        position += 4;
                        break;
                }

                position++;
            }
        }


        public static readonly TokenClass CloseBrace = new TokenClass("}");
        public static readonly TokenClass CloseBracket = new TokenClass("]");
        public static readonly TokenClass Colon = new TokenClass(":");
        public static readonly TokenClass Comma = new TokenClass(",");
        public static readonly TokenClass Error = new TokenClass("error");
        public static readonly TokenClass False = new TokenClass("false");
        public static readonly TokenClass Null = new TokenClass("null");
        public static readonly TokenClass Number = new TokenClass(@"[-+]?[0-9]*[.]?[0-9]+");
        public static readonly TokenClass OpenBrace = new TokenClass("{");
        public static readonly TokenClass OpenBracket = new TokenClass("[");
        public static readonly TokenClass String = new TokenClass("[\"][^\"]+[\"]");
        public static readonly TokenClass True = new TokenClass("true");
        public static readonly TokenClass Whitespace = new TokenClass(@"[\s]+");

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