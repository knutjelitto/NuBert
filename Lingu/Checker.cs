using System;
using System.Diagnostics;
using System.Globalization;
using Lingu.Tools;

namespace Lingu
{
    public class Checker
    {
        public void Check()
        {
            GenerateUnicodeCategories(false);
        }

        public void GenerateUnicodeCategories(bool write)
        {
            var sets = new IntegerSet[30];

            for (var i = 0; i < 30; ++i)
            {
                sets[i] = new IntegerSet();
            }
            
            for (int ch = char.MinValue; ch <= char.MaxValue; ++ch)
            {
                var category = (int)CharUnicodeInfo.GetUnicodeCategory((char) ch);
                Debug.Assert(category < 30);
                sets[category].Add(ch);
            }

            for (var i = 0; i < 30; ++i)
            {
                if (write)
                {
                    Console.WriteLine(
                        $"static {nameof(IntegerSet)} {(UnicodeCategory) i} = {nameof(IntegerSet)}.Parse(\"{sets[i]}\");");
                }
                else
                {
                    Console.WriteLine($"{(UnicodeCategory) i} = {sets[i].RangeCount} ranges");
                }
    }
        }
    }
}
