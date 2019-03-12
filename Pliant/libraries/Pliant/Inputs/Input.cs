using System;
using System.Diagnostics;

namespace Pliant.Inputs
{
    public class Input
    {
        public Input(string text)
        {
            this.text = text;
            Current = new Cursor(this.text);
        }

        public Cursor Current { get; private set; }

        public bool More => Current.More;
        public bool Valid => Current.Valid;

        public char Slurp()
        {
            Debug.Assert(Current.Valid);
            var c = Current.Value;
            Current = Current.Next();
            return c;
        }

        public void Advance()
        {
            Debug.Assert(Current.Valid);
            Current = Current.Next();
        }

        public bool Accept(Func<char, bool> predicate, ref char c)
        {
            if (predicate != null && Valid && predicate(Current))
            {
                c = Slurp();

                return true;
            }

            return false;
        }

        public bool Accept(char c)
        {
            if (Valid && Current == c)
            {
                Current = Current.Next();

                return true;
            }

            return false;
        }


        private readonly string text;
    }
}