using System.IO;
using System.Diagnostics;
using System;

namespace ItsMagic
{
    public static class Cauldron
    {
        public static void Add(string log)
        {
            var logFile = Dumbledore.MagicDir + @"\Cauldren\Log " + Process.GetCurrentProcess().StartTime.ToString().Replace("/","_").Replace(":", "_") + ".txt";
            if (!File.Exists(logFile))
               File.Create(logFile).Close();
            File.AppendAllText(logFile, log + Environment.NewLine);
            Console.WriteLine(log);
        }
    }
}
