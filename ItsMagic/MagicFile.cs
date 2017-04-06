using System;
using System.Collections.Generic;
using System.IO;

namespace Dumbledore
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

        public override bool Equals(object obj)
        {
            var other = obj as MagicFile;
            return Equals(other);
        }

        public bool Equals(MagicFile other)
        {
            if (other == null)
                return false;

            return FilePath == other.FilePath;
        }

        public override int GetHashCode()
        {
            return FilePath.GetHashCode();
        }

        public int CompareTo(MagicFile other)
        {
            return other == null ? 1 : FilePath.CompareTo(other.FilePath);
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
