using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dumbledore
{
    class PackagesConfig : MagicFile
    {
        private PackagesConfigEntry[] NugetpackageReferencesCache { get; set; }
        public PackagesConfigEntry[] NugetpackageReferences => NugetpackageReferencesCache ?? (NugetpackageReferencesCache = GetNugetPackageReferences());

        public PackagesConfig(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            FilePath = path;
        }

        private PackagesConfigEntry[] GetNugetPackageReferences()
        {
            List<PackagesConfigEntry> nugetPackageReferences = new List<PackagesConfigEntry>();
            foreach (var package in RegexStore.Get(RegexStore.PackageFromPackagesConfigPattern, Text))
            {
                nugetPackageReferences.Add(new PackagesConfigEntry(
                    RegexStore.Get(RegexStore.PackageIdFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageVersionFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageTargetFrameworkFromPackagesPattern, package).Single()));
            }

            return nugetPackageReferences.ToArray();
        }
    }
}
