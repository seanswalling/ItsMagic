using System.Linq;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            //var csFiles = Dumbledore.GetSlnFiles(@"C:\source\Mercury\src").SelectMany(Dumbledore.GetCsProjs);
            var csFiles = Dumbledore.GetSlnFiles(@"E:\github\cc\Mercury");
            foreach(CsFile csFile in csFiles)
            {
                CsFile 
            }
        }
    }
}
