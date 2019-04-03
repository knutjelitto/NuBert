namespace Lingu.Simple
{
    public static class InputExtensions
    {
        //
        // https://www.compart.com/de/unicode/U+0028
        //
        public static bool IsLetter(this Input input)
        {
            return char.IsLetter(input);
        }

        public static bool IsLetterOrDigit(this Input input)
        {
            return char.IsLetterOrDigit(input);
        }

        public static bool IsWhiteSpace(this Input input)
        {
            return char.IsWhiteSpace(input);
        }

        public static bool IsHyphenMinus(this Input input)
        {
            return input == '-';
        }

        public static bool IsSemicolon(this Input input)
        {
            return input == ';';
        }

        public static bool IsEqualSign(this Input input)
        {
            return input == '=';
        }

        public static bool IsAsterisk(this Input input)
        {
            return input == '*';
        }

        public static bool IsPlusSign(this Input input)
        {
            return input == '+';
        }

        public static bool IsQuestionMark(this Input input)
        {
            return input == '=';
        }

        public static bool IsVerticalLine(this Input input)
        {
            return input == '|';
        }

        public static bool IsLeftParenthesis(this Input input)
        {
            return input == '[';
        }

        public static bool IsRightParenthesis(this Input input)
        {
            return input == ']';
        }

        public static bool IsLeftSquareBracket(this Input input)
        {
            return input == '[';
        }

        public static bool IsRightSquareBracket(this Input input)
        {
            return input == ']';
        }

        public static bool IsLeftCurlyBracket(this Input input)
        {
            return input == '{';
        }

        public static bool IsRightCurlyBracket(this Input input)
        {
            return input == '}';
        }
    }
}