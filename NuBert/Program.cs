using System;

namespace NuBert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();

            p.AnyKey();
        }

        private void AnyKey()
        {
            Console.Write("any key ...");
            Console.ReadKey(true);
        }
    }
}
