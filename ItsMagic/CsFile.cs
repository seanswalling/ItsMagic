using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ItsMagic
{
    public class CsFile
    {
        public string Path { get; private set; }
        public string[] Classes { get; private set; }
        public string[] Usings { get; private set; }
        private string _textCache { get; set; }

        public CsFile(string path)
        {
            Path = path;
            Classes = RegexStore.Get(RegexStore.ClassFromCsFile, Path).ToArray();
            Usings = RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Path).ToArray();
        }

        public string Text()
        {
            if (_textCache != null)
                return _textCache;
            var text = File.ReadAllText(Path);
            return text;
        }
        
        public void AddUsing(string reference)
        {
            if (!Text().Contains("using " + reference + ";"))
            {
                string newText = "using " + reference + ";"+ Environment.NewLine + Text();
                File.WriteAllText(Path, newText);
            }
        }

        public void RemoveUsing(string reference)
        {
            if (Text().Contains("using " + reference + ";"))
            {
                var replace = Text().Replace("using " + reference + ";"+Environment.NewLine, "");
                File.WriteAllText(Path, replace);
            }
        }

        public void AlphabatiseUsings()
        {
            foreach(var @using in Usings)
            {
                RemoveUsing(@using);
            }
            foreach(var @using in Usings.OrderByDescending(u => u))
            {
                AddUsing(@using);
            }
        }

        public IEnumerable<string> Lines()
        {
            using (var reader = new StreamReader(Path))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        public bool HasEvidenceOf(CsProj csProj)
        {
            foreach (var csProjClass in csProj.Classes)
            {
                if (RegexStore.Contains("[\\s:]" + csProjClass + "[\\s\\.(]", Text()))
                {
                    return true;
                }
            }
            //Foreach(Extension Method in CsProj.ExtensionMethods)
            return false;
        }

        //Functions to be Deprecated

        public bool HasEvidenceOfJExt()
        {
            return Text().Contains(".JsonCopy()")
                   || Text().Contains(".ToBson()")
                   || Text().Contains("SettingsFactory.Build()")
                   || Text().Contains("DateSerializer")
                   || Text().Contains("RequiredPropertyContractResolver")
                   || Text().Contains(".FromBson()");
        }

        public bool HasEvidenceOfNHibExt()
        {
            return Text().Contains(".Nullable()")
                   || Text().Contains(".NotNullable()")
                   && !Path.Contains("Mercury.Core.NHibernateExtensions.cs");
        }

        public bool HasEvidenceOfLogRepoSc()
        {
            return (Text().Contains("ISharedAccessKeyService")
                   || Text().Contains("IPrimeService"))
                   && !Path.Contains("ISharedAccessKeyService.cs")
                   && !Path.Contains("IPrimeService.cs");
        }                

        public bool HasEvidenceOfWeTc()
        {
            return (Text().Contains("AggregateEx")
                   || Text().Contains("AlterDepencencies")
                   || Text().Contains("ResolutionInfo")
                   || Text().Contains("BufferingScenarioRenderer")
                   || Text().Contains("ChainedScenario")
                   || Text().Contains("ChainedScenarios")
                   || Text().Contains("CommandWriterTestExtensions")
                   || Text().Contains("EnvironmentScenarioRenderer")
                   || Text().Contains("EventAssert")
                   || Text().Contains("EventWriterTestExtensions")
                   || Text().Contains("HandlerTest")
                   || Text().Contains("InMemoryDatabaseFixtureBase")
                   || Text().Contains("IScenarioRender")
                   || Text().Contains("LocalDbConfiguration")
                   || Text().Contains("LocalDbDatabaseFixture")
                   || Text().Contains("MessageStoreTestExtensions")
                   || Text().Contains("MessageHubWriterExtensions")
                   || Text().Contains("ProcessManagerStructureValidationFixtureBase")
                   || Text().Contains("ProdScenario")
                   || Text().Contains("RegressionScenarios")
                   || Text().Contains("Scenario")
                   || Text().Contains("ScenarioCollection")
                   || Text().Contains("ScenarioDrivenTestEnvironment")
                   || Text().Contains("ScenarioDrivenTestService")
                   || Text().Contains("ScenarioEx")
                   || Text().Contains("ScenarioOutputVisitor")
                   || Text().Contains("ScenarioRenderer")
                   || Text().Contains("SetCulture")
                   || Text().Contains("SetTime")
                   || Text().Contains("SharedScenarioAssertions")
                   || Text().Contains("SimulatedCommand")
                   || Text().Contains("SimulatedDocument")
                   || Text().Contains("SimulatedEvent")
                   || Text().Contains("SimulatedFastForward")
                   || Text().Contains("StoppableServiceWrapper")
                   || Text().Contains("TransientExceptionSideEffect"))
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