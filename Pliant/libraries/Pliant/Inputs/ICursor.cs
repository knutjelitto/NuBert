using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Inputs
{
    public interface ICursor
    {
        int Position { get; }
        char Value { get; }
        bool Valid { get; }
        bool More { get; }
        ICursor Next();

        string Upto(Input end);
    }
}
