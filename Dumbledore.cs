using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    static class Dumbledore
    {
        static public void UpdateProjectReferenceWithNugetReference(string csProjFileToUpdate, CsProj projectReferenceToRemove,
            NugetPackageReference referenceToAdd)
        {
            
        }








        static public IEnumerable<string> GetSlnFiles(string projectDirectory)
        {
            return Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories);
        }

        static public IEnumerable<string> GetCsProjs(string sln)
        {
            var dir = Path.GetDirectoryName(sln);
            var regex = new Regex("Project(.*) = .*, \"(?<proj>.*\\.csproj)\", \".*\"");
            return File.ReadAllLines(sln)
                .Select(f => regex.Match(f))
                .Where(m => m.Success)
                .Select(m => m.Groups["proj"].Value)
                .Select(relPath => Path.Combine(dir, relPath));
        }

        static public IEnumerable<string> GetCsFiles(string csproj)
        {
            var dir = Path.GetDirectoryName(csproj);
            var regex = new Regex("(?:<Compile Include=\\\")(?<cs>(?:(?:\\w+)\\\\*(?:\\w+))*\\.cs)(?:\\\" \\/>)");
            return File.ReadAllLines(csproj)
                .Select(f => regex.Match(f))
                .Where(m => m.Success)
                .Select(m => m.Groups["cs"].Value)
                .Select(relPath => Path.Combine(dir, relPath));
        }
    }
}
