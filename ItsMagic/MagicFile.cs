using System;
using System.IO;

namespace ItsMagic
{
    public class MagicFile : IEquatable<MagicFile>, IComparable<MagicFile>
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

        public bool Equals(MagicFile other)
        {
            if (other == null)
                return false;

            if (this.Path == other.Path)
                return true;
            else
                return false;
        }

        public int CompareTo(MagicFile other)
        {
            if (other == null)
                return 1;

            return Path.CompareTo(other.Path);
        }
    }
}
