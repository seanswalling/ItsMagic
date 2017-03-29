using System;
using System.IO;

namespace ItsMagic
{
    public abstract class MagicFile : IEquatable<MagicFile>, IComparable<MagicFile>
    {
        public string FilePath { get; set; }
        private string TextCache { get; set; }

        public string Text
        {
            get { return TextCache ?? (TextCache = File.ReadAllText(FilePath)); }
            set {  TextCache = value; }
        } 
        public string Name => Path.GetFileNameWithoutExtension(FilePath);

        public void WriteFile()
        {
            File.WriteAllText(FilePath, Text);
        }

        public bool Equals(MagicFile other)
        {
            if (other == null)
                return false;

            return this.FilePath == other.FilePath;
        }

        public int CompareTo(MagicFile other)
        {
            return other == null ? 1 : FilePath.CompareTo(other.FilePath);
        }
    }
}
