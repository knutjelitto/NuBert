using System;
using System.Data;
using System.Xml;

namespace Lingu.Simple
{
    public class Scanner
    {
        public Scanner(Input input)
        {
            this.Input = input;
        }

        public Input Input { get; private set; }

        public static readonly STerminal Identifier = new STerminal("identifier");
        public static readonly STerminal Semicolon = new STerminal(";");
        public static readonly STerminal EqualSign = new STerminal("=");
        public static readonly STerminal Asterisk = new STerminal("*");
        public static readonly STerminal PlusSign = new STerminal("+");
        public static readonly STerminal QuestionMark = new STerminal("?");
        public static readonly STerminal Verticalline = new STerminal("|");
        public static readonly STerminal LeftParenthesis = new STerminal("(");
        public static readonly STerminal RightParenthesis = new STerminal(")");
        public static readonly STerminal LeftSquareBracket = new STerminal("[");
        public static readonly STerminal RightSquareBracket = new STerminal("]");
        public static readonly STerminal LeftCurlyBracket = new STerminal("{");
        public static readonly STerminal RightCurlyBracket = new STerminal("}");

        public SToken Next()
        {
            SkipWhiteSpace();

            if (!Input)
            {
                return null;
            }

            switch (Input)
            {
                case ';':
                    return MakeToken(Semicolon);
                case '=':
                    return MakeToken(EqualSign);
                case '*':
                    return MakeToken(Asterisk);
                case '+':
                    return MakeToken(PlusSign);
                case '?':
                    return MakeToken(QuestionMark);
                case '|':
                    return MakeToken(Verticalline);
                case '(':
                    return MakeToken(LeftParenthesis);
                case ')':
                    return MakeToken(RightParenthesis);
                case '[':
                    return MakeToken(LeftSquareBracket);
                case ']':
                    return MakeToken(RightSquareBracket);
                case '{':
                    return MakeToken(LeftCurlyBracket);
                case '}':
                    return MakeToken(RightCurlyBracket);
            }

            if (Input.IsLetter())
            {
                var start = Input;
                Input = Input.Next;
                var hype = true;
                while (hype)
                {
                    hype = false;
                    while (Input.IsLetterOrDigit())
                    {
                        Input = Input.Next;
                    }
                    while (Input.IsHyphenMinus())
                    {
                        hype = true;
                        Input = Input.Next;
                    }
                    if (hype && !Input.IsLetterOrDigit())
                    {
                        if (Input)
                        {
                            throw new ScannerException($"expected letter or digit in identifier but got '{Input.Value}'");
                        }
                        throw new ScannerException($"expected letter or digit in identifier but got <eof>");
                    }
                }

                return MakeToken(Identifier, start);
            }

            throw new ScannerException($"unexpected character '{Input.Value}'");
        }

        private SToken MakeToken(STerminal terminal, Input start)
        {
            return new SToken(terminal, start, Input);
        }

        private SToken MakeToken(STerminal terminal)
        {
            var start = Input;
            Input = Input.Next;
            return MakeToken(terminal, start);
        }

        private void SkipSingleComment()
        {
            while (Input.IsNot('\n'))
            {
                Input = Input.Next;
            }
        }

        private void SkipMultiComment()
        {
            while (Input)
            {
                while (Input.IsNot('*'))
                {
                    Input = Input.Next;
                }
                if (!Input)
                {
                    throw new ScannerException("<eof> in multi line comment");
                }
                if (Input.Is('/'))
                {
                    Input = Input.Next;
                    return;
                }
                Input = Input.Next;
            }
        }

        private void SkipWhiteSpace()
        {
            while (Input)
            {
                if (Input.IsWhiteSpace())
                {
                    Input = Input.Next;
                    continue;
                }
                if (Input && Input == '/')
                {
                    var next = Input.Next;
                    if (next)
                    {
                        if (next == '/')
                        {
                            Input = next.Next;
                            SkipSingleComment();
                        }
                        else if (next == '*')
                        {
                            Input = next.Next;
                            SkipMultiComment();
                        }
                    }
                }
                break;
            }

        }
    }
}