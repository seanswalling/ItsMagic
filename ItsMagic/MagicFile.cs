using System;
using System.Collections.Generic;
using System.IO;

namespace Dumbledore
{
    public abstract class MagicFile : IEquatable<MagicFile>, IComparable<MagicFile>
    {
        public bool Exists { get; }

        protected MagicFile(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                Text = File.ReadAllText(FilePath);
            }

            Exists = true;
            if (!File.Exists(FilePath))
                Exists = false;
        }

        public string FilePath { get; }

        public string Text { get ; set; }

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
