using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            string dir = "C:\\source\\Mercury\\src";
            Dumbledore.AddJExtAndNHibExtReferences(dir);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
    }
}
