using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            var csFiles = Dumbledore.GetSlnFiles(@"C:\source\Mercury\src").SelectMany(Dumbledore.GetCsProjs);

        }
    }
}
