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

                    yield return new Token(position, builder.ToString(), Number);

                    position += builder.Length;
                    builder.Clear();
                    continue;
                }

                switch (c)
                {
                    case '[':
                        yield return new Token(position, OpenBracket.Id, OpenBracket);
                        break;

                    case ']':
                        yield return new Token(position, CloseBracket.Id, CloseBracket);
                        break;

                    case '{':
                        yield return new Token(position, OpenBrace.Id, OpenBrace);
                        break;

                    case '}':
                        yield return new Token(position, CloseBrace.Id, CloseBrace);
                        break;

                    case ',':
                        yield return new Token(position, Comma.Id, Comma);
                        break;

                    case ':':
                        yield return new Token(position, Colon.Id, Colon);
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
                        yield return new Token(position, builder.ToString(), String);

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
                        yield return new Token(position, builder.ToString(), Whitespace);

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
                        yield return new Token(position, Null.Id, Null);

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
                        yield return new Token(position, True.Id, True);

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

                        yield return new Token(position, False.Id, False);

                        position += 4;
                        break;
                }

                position++;
            }
        }


        public static readonly TokenType CloseBrace = new TokenType("}");
        public static readonly TokenType CloseBracket = new TokenType("]");
        public static readonly TokenType Colon = new TokenType(":");
        public static readonly TokenType Comma = new TokenType(",");
        public static readonly TokenType Error = new TokenType("error");
        public static readonly TokenType False = new TokenType("false");
        public static readonly TokenType Null = new TokenType("null");
        public static readonly TokenType Number = new TokenType(@"[-+]?[0-9]*[.]?[0-9]+");
        public static readonly TokenType OpenBrace = new TokenType("{");
        public static readonly TokenType OpenBracket = new TokenType("[");
        public static readonly TokenType String = new TokenType("[\"][^\"]+[\"]");
        public static readonly TokenType True = new TokenType("true");
        public static readonly TokenType Whitespace = new TokenType(@"[\s]+");

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