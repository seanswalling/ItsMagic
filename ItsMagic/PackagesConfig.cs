using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ItsMagic
{
    class PackagesConfig : MagicFile
    {
        private NugetPackageReference[] NugetpackageReferencesCache { get; set; }
        public NugetPackageReference[] NugetpackageReferences => NugetpackageReferencesCache ?? (NugetpackageReferencesCache = GetNugetPackageReferences());

        public PackagesConfig(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            FilePath = path;
        }

        private NugetPackageReference[] GetNugetPackageReferences()
        {
            List<NugetPackageReference> nugetPackageReferences = new List<NugetPackageReference>();
            foreach (var package in RegexStore.Get(RegexStore.PackageFromPackagesConfigPattern, Text))
            {
                nugetPackageReferences.Add(new NugetPackageReference(
                    RegexStore.Get(RegexStore.PackageIdFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageVersionFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageTargetFrameworkFromPackagesPattern, package).Single()));
            }

            return nugetPackageReferences.ToArray();
        }
    }
}
