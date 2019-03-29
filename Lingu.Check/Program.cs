using System;
using System.Text;

namespace Lingu.Check
{
    internal static class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            new Checker().Check();

            AnyKey();
        }


        private static void AnyKey()
        {
            Console.Write("any key ...");
            Console.ReadKey(true);
        }
    }
}
