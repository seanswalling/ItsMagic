namespace Dumbledore
{
    public class NugetPackageReference
    {
        private PackagesConfigEntry PackagesConfigEntry { get; }
        public string Id { get; }
        public string Version => PackagesConfigEntry.Version;
        public string TargetFramework => PackagesConfigEntry.TargetFramework;
        public string HintPath { get; set; }
        public string Include { get; set; }

        public NugetPackageReference(string hintPath, string include, PackagesConfigEntry packagesConfigEntry)
        {
            HintPath = hintPath;
            PackagesConfigEntry = packagesConfigEntry;
            Include = include;
        }

        public NugetPackageReference(string id, string hintPath, string include)
        {
            HintPath = hintPath;
            Include = include;
            Id = id;
        }
    }
}
