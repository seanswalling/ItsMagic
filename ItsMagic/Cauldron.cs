using System;
using System.Diagnostics;
using System.IO;

namespace Dumbledore
{
    public static class Cauldron
    {
        public static void Add(string log)
        {
            if (!Directory.Exists(Wand.MagicDir + @"\Cauldren"))
                Directory.CreateDirectory(Wand.MagicDir + @"\Cauldren");

            var logFile = Wand.MagicDir + @"\Cauldren\Log " + Process.GetCurrentProcess().StartTime.ToString().Replace("/","_").Replace(":", "_") + ".txt";

            if (!File.Exists(logFile))
               File.Create(logFile).Close();

            File.AppendAllText(logFile, log + Environment.NewLine);
            Console.WriteLine(log);
        }
    }
}
