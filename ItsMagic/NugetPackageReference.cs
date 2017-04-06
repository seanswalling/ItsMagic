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
        public string DllName { get; set; }

        public NugetPackageReference(string id, string dllName, string hintPath, string include, PackagesConfigEntry packagesConfigEntry)
        {
            HintPath = hintPath;
            PackagesConfigEntry = packagesConfigEntry;
            Include = include;
            Id = id;
            DllName = dllName;
        }

        public override string ToString()
        {
            return DllName;
        }
    }
}
