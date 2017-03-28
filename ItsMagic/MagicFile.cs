using System;
using System.IO;

namespace ItsMagic
{
    public class MagicFile : IEquatable<MagicFile>, IComparable<MagicFile>
    {
        public string Filepath { get; set; }
        private string TextCache { get; set; }

        public string Text
        {
            get { return TextCache ?? (TextCache = File.ReadAllText(Filepath)); }
            set {  TextCache = value; }
        } 
        public string Name => Path.GetFileNameWithoutExtension(Filepath);

        public void WriteText(string text)
        {
            File.WriteAllText(Filepath, Text);
        }

        public bool Equals(MagicFile other)
        {
            if (other == null)
                return false;

            return this.Filepath == other.Filepath;
        }

        public int CompareTo(MagicFile other)
        {
            return other == null ? 1 : Filepath.CompareTo(other.Filepath);
        }
    }
}
