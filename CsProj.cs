namespace ItsMagic
{
    class CsProj
    {
        public string Guid { get; set; }
        public string OutputType { get; set; }
        public string RootNamespace { get; set; }
        public string TargetFrameworkVersion { get; set; }
        public string [] NugetPackageReferences { get; set; }
        public CsProj [] ProjectReferences { get; set; }
        public CsFile [] CsFiles { get; set; }
    }
}
