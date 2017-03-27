using System.IO;

namespace ItsMagic
{
    public class MagicFile
    {
        public string Path { get; set; }
        public string TextCache { get; set; }
        public string Text
        {
            get
            {
                return TextCache ?? (TextCache = File.ReadAllText(Path));
            }
        }
        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);


        
        public void WriteText(string newText)
        {
            TextCache = newText;
            File.WriteAllText(Path, newText);
        }
    }
}
