using System;
using System.Text;

namespace NuBert
{
    internal static class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

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
