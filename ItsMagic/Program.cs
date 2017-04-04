using System;
using System.Diagnostics;

namespace Dumbledore
{
    class Program
    {
        static void Main()
        {
            //TODO transform this whole app into a class lib and make a cli then make a gui.
            var sw = Stopwatch.StartNew();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
    }
}
