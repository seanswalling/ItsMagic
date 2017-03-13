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
            //string dir = "C:\\source\\Mercury\\src";
            string dir = @"E:\github\cc\Mercury\src";
            Dumbledore.AddJExtAndNHibExtReferences(dir);
            //Dumbledore.FixXML(dir);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
    }
}
