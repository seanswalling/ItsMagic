using System.IO;

namespace ItsMagic
{
    public class MagicFile
    {
        public string Path { get; set; }
        private string _textCache { get; set; }

        public string Text()
        {
            if (_textCache == null)
                _textCache = File.ReadAllText(Path);
            return _textCache;
        }
        
        public void WriteText(string newText)
        {
            _textCache = newText;
            File.WriteAllText(Path, newText);
        }
    }
}
