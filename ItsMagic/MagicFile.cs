using System.IO;

namespace ItsMagic
{
    public class MagicFile
    {
        public string Path { get; set; }
        public string TextCache { get; set; }
        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public string Text()
        {
            if (TextCache == null)
                TextCache = File.ReadAllText(Path);
            return TextCache;
        }
        
        public void WriteText(string newText)
        {
            TextCache = newText;
            File.WriteAllText(Path, newText);
        }
    }
}
