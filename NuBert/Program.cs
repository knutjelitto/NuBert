using System;
using Lingu;

namespace NuBert
{
    internal static class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var checker = new Checker();

            checker.Check();

            AnyKey();
        }


        private static void AnyKey()
        {
            Console.Write("any key ...");
            Console.ReadKey(true);
        }
    }
}
