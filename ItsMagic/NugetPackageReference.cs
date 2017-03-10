namespace ItsMagic
{
    public class NugetPackageReference
    {
        public string Include { get; set; }
        public string HintPath { get; set; }
        public string SpecificVersion { get; set; }
        public string Private { get; set; }
        public string Value { get; set; }
        public string Pattern { get; internal set; }
    }
}