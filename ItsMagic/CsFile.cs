using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ItsMagic
{
    public class CsFile
    {
        public string Path { get; private set; }
        public string[] Classes { get; private set; }

        public CsFile(string path)
        {
            Path = path;
            Classes = GetClasses();
        }

        private string[] GetClasses()
        {
            return RegexStore.Get(RegexStore.ClassFromCsFile, Path).ToArray();
        }

        public IEnumerable<string> Usings()
        {
            Console.WriteLine("Get Using Statements for: " + Path);
            return RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Path);
        }

        public IEnumerable<string> GetLines()
        {
            using (var reader = new StreamReader(Path))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        public bool HasEvidenceOfJExt()
        {
            var csFileText = File.ReadAllText(Path);
            return csFileText.Contains(".JsonCopy()")
                   || csFileText.Contains(".ToBson()")
                   || csFileText.Contains("SettingsFactory.Build()")
                   || csFileText.Contains("DateSerializer")
                   || csFileText.Contains("RequiredPropertyContractResolver")
                   || csFileText.Contains(".FromBson()");
        }

        public bool HasEvidenceOfNHibExt()
        {
            var csFileText = File.ReadAllText(Path);
            return csFileText.Contains(".Nullable()")
                   || csFileText.Contains(".NotNullable()")
                   && !Path.Contains("Mercury.Core.NHibernateExtensions.cs");
        }

        public bool HasEvidenceOfLogRepoSc()
        {
            var csFileText = File.ReadAllText(Path);
            return (csFileText.Contains("ISharedAccessKeyService")
                   || csFileText.Contains("IPrimeService"))
                   && !Path.Contains("ISharedAccessKeyService.cs")
                   && !Path.Contains("IPrimeService.cs");
        }

        public void AddUsingToCsFile(string reference)
        {
            if (!File.ReadAllText(Path).Contains("using " + reference))
            {
                var csFileText = File.ReadAllText(Path);
                csFileText = "using " + reference + ";\r" + csFileText;
                File.WriteAllText(Path, csFileText);
            }
        }

        public void RemoveUsing(string reference)
        {
            var csFileText = File.ReadAllText(Path);
            if (csFileText.Contains("using " + reference))
            {
                var replace = csFileText.Replace("using " + reference + ";\r", "");
                File.WriteAllText(Path, replace);
            }
        }

        public bool HasEvidenceOf(CsProj csProj)
        {
            var csFiles = csProj.CsFiles;
            var allClassesInCsProj = csFiles.SelectMany(csFile => csFile.Classes).Distinct();
            var csFileText = File.ReadAllText(Path);
            foreach (var csProjClass in allClassesInCsProj)
            {
                if(RegexStore.Contains("[\\s:]"+csProjClass+ "[\\s\\.(]", csFileText))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasEvidenceOfWeTc()
        {
            var csFileText = File.ReadAllText(Path);
            return (csFileText.Contains("AggregateEx")
                   || csFileText.Contains("AlterDepencencies")
                   || csFileText.Contains("ResolutionInfo")
                   || csFileText.Contains("BufferingScenarioRenderer")
                   || csFileText.Contains("ChainedScenario")
                   || csFileText.Contains("ChainedScenarios")
                   || csFileText.Contains("CommandWriterTestExtensions")
                   || csFileText.Contains("EnvironmentScenarioRenderer")
                   || csFileText.Contains("EventAssert")
                   || csFileText.Contains("EventWriterTestExtensions")
                   || csFileText.Contains("HandlerTest")
                   || csFileText.Contains("InMemoryDatabaseFixtureBase")
                   || csFileText.Contains("IScenarioRender")
                   || csFileText.Contains("LocalDbConfiguration")
                   || csFileText.Contains("LocalDbDatabaseFixture")
                   || csFileText.Contains("MessageStoreTestExtensions")
                   || csFileText.Contains("MessageHubWriterExtensions")
                   || csFileText.Contains("ProcessManagerStructureValidationFixtureBase")
                   || csFileText.Contains("ProdScenario")
                   || csFileText.Contains("RegressionScenarios")
                   || csFileText.Contains("Scenario")
                   || csFileText.Contains("ScenarioCollection")
                   || csFileText.Contains("ScenarioDrivenTestEnvironment")
                   || csFileText.Contains("ScenarioDrivenTestService")
                   || csFileText.Contains("ScenarioEx")
                   || csFileText.Contains("ScenarioOutputVisitor")
                   || csFileText.Contains("ScenarioRenderer")
                   || csFileText.Contains("SetCulture")
                   || csFileText.Contains("SetTime")
                   || csFileText.Contains("SharedScenarioAssertions")
                   || csFileText.Contains("SimulatedCommand")
                   || csFileText.Contains("SimulatedDocument")
                   || csFileText.Contains("SimulatedEvent")
                   || csFileText.Contains("SimulatedFastForward")
                   || csFileText.Contains("StoppableServiceWrapper")
                   || csFileText.Contains("TransientExceptionSideEffect"))
                   && !Path.Contains("AggregateEx.cs")
                   && !Path.Contains("AlterDepencencies.cs")
                   && !Path.Contains("ResolutionInfo.cs")
                   && !Path.Contains("BufferingScenarioRenderer.cs")
                   && !Path.Contains("ChainedScenario.cs")
                   && !Path.Contains("ChainedScenarios.cs")
                   && !Path.Contains("CommandWriterTestExtensions.cs")
                   && !Path.Contains("EnvironmentScenarioRenderer.cs")
                   && !Path.Contains("EventAssert.cs")
                   && !Path.Contains("EventWriterTestExtensions.cs")
                   && !Path.Contains("HandlerTest.cs")
                   && !Path.Contains("InMemoryDatabaseFixtureBase.cs")
                   && !Path.Contains("IScenarioRender.cs")
                   && !Path.Contains("LocalDbConfiguration.cs")
                   && !Path.Contains("LocalDbDatabaseFixture.cs")
                   && !Path.Contains("MessageStoreTestExtensions.cs")
                   && !Path.Contains("MessageHubWriterExtensions.cs")
                   && !Path.Contains("ProcessManagerStructureValidationFixtureBase.cs")
                   && !Path.Contains("ProdScenario.cs")
                   && !Path.Contains("RegressionScenarios.cs")
                   && !Path.Contains("Scenario.cs")
                   && !Path.Contains("ScenarioCollection.cs")
                   && !Path.Contains("ScenarioDrivenTestEnvironment.cs")
                   && !Path.Contains("ScenarioDrivenTestService.cs")
                   && !Path.Contains("ScenarioEx.cs")
                   && !Path.Contains("ScenarioOutputVisitor.cs")
                   && !Path.Contains("ScenarioRenderer.cs")
                   && !Path.Contains("SetCulture.cs")
                   && !Path.Contains("SetTime.cs")
                   && !Path.Contains("SharedScenarioAssertions.cs")
                   && !Path.Contains("SimulatedCommand.cs")
                   && !Path.Contains("SimulatedDocument.cs")
                   && !Path.Contains("SimulatedEvent.cs")
                   && !Path.Contains("SimulatedFastForward.cs")
                   && !Path.Contains("StoppableServiceWrapper.cs")
                   && !Path.Contains("TransientExceptionSideEffect.cs");
        }
    }
}