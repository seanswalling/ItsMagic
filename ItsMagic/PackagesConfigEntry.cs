namespace Dumbledore
{
    public class PackagesConfigEntry
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }

        public PackagesConfigEntry(string id, string version, string targetFramework)
        {
            Id = id;
            Version = version;
            TargetFramework = targetFramework;
        }
    }
}