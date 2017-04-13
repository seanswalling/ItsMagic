using System;
using System.IO;
using System.Linq;

namespace Dumbledore
{
    public abstract class MagicFile : IComparable<MagicFile>
    {
        public bool Exists { get; }
        protected MagicFile(string filePath)
        {
            Filepath = filePath;
            if (File.Exists(FilePath))
            {
            Text = File.ReadAllText(Filepath);
            }

            Exists = true;
            if (!File.Exists(FilePath))
                Exists = false;
        }

        public string Filepath { get; }

        public string Text { get ; set; }

        public string Name => Path.GetFileNameWithoutExtension(Filepath);

        public void WriteFile()
        {
            File.WriteAllText(Filepath, Text);
        }

        public int CompareTo(MagicFile other)
        {
            return other == null ? 1 : string.Compare(Filepath, other.Filepath, StringComparison.Ordinal);
        }
        /// <summary>
        /// Read the file as an array, removes the given line numbers then rewrites
        /// the array before refreshing the Text property.
        /// </summary>
        /// <param name="lineNumbersToRemove"></param>
        public void RemoveLines(int[] lineNumbersToRemove)
        {
            var content = File.ReadAllLines(Filepath);
            content = content.Where((line, index) => !lineNumbersToRemove.Contains(index + 1)).ToArray();
            File.WriteAllLines(Filepath, content);
            Text = File.ReadAllText(Filepath);
        }
    }
}
