namespace ItsMagic
{
    public class NugetPackageReference
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }

        public NugetPackageReference(string id, string version, string targetFramework)
        {
            Id = id;
            Version = version;
            TargetFramework = targetFramework;
        }
    }
}