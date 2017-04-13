using System.IO;
using System.Linq;
using Dumbledore;

namespace RemoveLogRepo
{
    public static class Steps
    {
        static void Main()
        {
            Run();
        }

        private static void Run()
        {
            Step1.Run();
            Step2.Run();
            Step3.Run();
            Step4.Run();
        }

        internal static class Step1
        {
            private static readonly CsFile Lfn = new CsFile(@"C:\source\Mercury\src\Platform\Server.Core\Logging\Log4NetConfiguration.cs");
            private static readonly CsFile Tl = new CsFile(@"C:\source\Mercury\src\Terminal\Terminal\Logging\TerminalLogging.cs");
            private static readonly CsProj Terminalproj = CsProj.Get(@"C:\source\Mercury\src\Terminal\Terminal\Terminal.csproj");
            private static readonly CsProj Servercoreproj = CsProj.Get(@"C:\source\Mercury\src\Platform\Server.Core\Server.Core.csproj");
            private static readonly SlnFile Terminalsln = new SlnFile(@"C:\source\Mercury\src\Mercury.Terminal.sln");
            private static readonly SlnFile Platformsln = new SlnFile(@"C:\source\Mercury\src\Mercury.Platform.sln");
            private const string LogRepoServiceContracts = "7A7CE935-5AF5-477E-A361-C8AB18C4FBED";
            private const string LoggingClient = "42A388A2-B797-4335-8A7D-8D748F58E7A3";

            internal static void Run()
            {
                Editlfn();
                EditTermLog();
                RemoveRefInCsProjs();
                RemoveRefInSlns();
            }

            private static void Editlfn()
            {
                var buildLogRepoMethod = Enumerable.Range(265, 11).ToArray();
                var definLogRepoMethod = Enumerable.Range(276, 6).ToArray();
                var linesToRemove = new int[] { 15, 120, 129, 130, 131, 132, 145, 151, 156, 173, 182, 187, 193 }.Concat(buildLogRepoMethod).Concat(definLogRepoMethod).ToArray();
                Lfn.ReadLines();
                Lfn.RemoveLines(linesToRemove);
                Lfn.WriteLines();
            }

            private static void EditTermLog()
            {
                var buildLogrepoMethod = Enumerable.Range(133, 11).ToArray();
                var lineToRemove = new int[] { 1, 10, 45, 51, 57 }.Concat(buildLogrepoMethod).ToArray();
                Tl.ReadLines();
                Tl.RemoveLines(lineToRemove);
                Tl.WriteLines();
            }

            private static void RemoveRefInCsProjs()
            {
                Terminalproj.RemoveProjectReference(LoggingClient);
                Terminalproj.RemoveProjectReference(LogRepoServiceContracts);
                Servercoreproj.RemoveProjectReference(LoggingClient);
                Servercoreproj.RemoveProjectReference(LogRepoServiceContracts);
            }

            private static void RemoveRefInSlns()
            {
                Terminalsln.RemoveProjectReference(LoggingClient);
                Terminalsln.RemoveProjectReference(LogRepoServiceContracts);
                Platformsln.RemoveProjectReference(LoggingClient);
                Platformsln.RemoveProjectReference(LogRepoServiceContracts);
            }
        }

        internal static class Step2
        {
            private const string LogRepoServiceContracts = "7A7CE935-5AF5-477E-A361-C8AB18C4FBED";
            private const string LoggingClient = "42A388A2-B797-4335-8A7D-8D748F58E7A3";
            //Remove Refs in all other projects to client csproj and log repo

            internal static void Run()
            {
                RemoveProjectRefs();
                RemoveSlnRefs();
            }

            private static void RemoveProjectRefs()
            {
                var projs = Wand.GetCsProjFiles(@"C:\source\Mercury\src");
                foreach (var proj in projs)
                {
                    proj.RemoveProjectReference(LogRepoServiceContracts);
                    proj.RemoveProjectReference(LoggingClient);
                }
            }

            private static void RemoveSlnRefs()
            {
                var slns = Wand.GetSolutionFiles(@"C:\source\Mercury\src");
                foreach (var sln in slns)
                {
                    sln.RemoveProjectReference(LogRepoServiceContracts);
                    sln.RemoveProjectReference(LoggingClient);
                }
            }
        }

        internal static class Step3
        {
            //Remove Log Repo Folder, Logging Client, Fix Reference in configuration generator
            private const string LogRepoFolder = @"C:\source\Mercury\src\LogRepository";
            private const string LoggingClientFolder = @"C:\source\Mercury\src\Platform\Logging.Client";
            private const string LogRepoSln = @"C:\source\Mercury\src\Mercury.LogRepository.sln";
            private static readonly CsProj ConfigGenerator = CsProj.Get(@"C:\source\Mercury\src\InitialState\ConfigurationGenerator\ConfigurationGenerator.csproj");
            private static readonly CsFile Program = new CsFile(@"C:\source\Mercury\src\InitialState\ConfigurationGenerator\Program.cs");

            internal static void Run()
            {
                DeleteLogRepo();
                DeleteLoggingClient();
                FixConfigGen();
            }

            private static void DeleteLogRepo()
            {
                Directory.Delete(LogRepoFolder, true);
                File.Delete(LogRepoSln);
            }

            private static void DeleteLoggingClient()
            {
                Directory.Delete(LoggingClientFolder, true);
            }

            private static void FixConfigGen()
            {
                ConfigGenerator.RemoveCompileEntry("LogRepositoryEnvironmentConfiguration.cs");
                int[] configBuilder = Enumerable.Range(770, 12).ToArray();
                int[] linestoremove = new int[] { 82 }.Concat(configBuilder).ToArray();
                Program.ReadLines();
                Program.RemoveLines(linestoremove);
                Program.WriteLines();
            }

        }

        internal static class Step4
        {
            //Remove log repo from system structure tests
            private static readonly CsFile Ss = new CsFile(@"C:\source\Mercury\src\Common\SystemDescription\SystemStructure.cs");

            internal static void Run()
            {
                Removeservicedesc();
            }

            private static void Removeservicedesc()
            {
                var servicedesc = Enumerable.Range(2321, 51).ToArray();
                Ss.ReadLines();
                Ss.RemoveLines(servicedesc);
                Ss.WriteLines();
            }
        }
    }


}
