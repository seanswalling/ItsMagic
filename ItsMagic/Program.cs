using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            Stopwatch sw = Stopwatch.StartNew();
            //string dir = "C:\\source\\Mercury\\src";
            string dir = @"E:\github\cc\Mercury\src";

            //Dumbledore.AddJExtAndNHibExtReferences(dir);
            //Dumbledore.RemoveLogForNetReference(FilesToFix());
            //Dumbledore.FixNHibExtUsings(Directory.EnumerateFiles(dir,"*.cs",SearchOption.AllDirectories).ToArray());
            //Dumbledore.AddNewRelicRefsTo(FilesThatRequireNewRelic());
            //PrintcsProjsDependantOnlogRepoSc(dir);
            //UpdateLogRepositoryPaths(dir);

            Dumbledore.AddProjectReferences(dir, new CsProj(@"C:\source\Mercury\src\Platform\WorkerEngine.TestCommon\WorkerEngine.TestCommon.csproj"));

            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }

        private static void UpdateLogRepositoryPaths(string dir)
        {
            foreach (var slnFile in Dumbledore.GetSolutionFiles(dir).ToArray())
            {
                foreach (var csProj in slnFile.CsProjs()
                    .Where(csProj => csProj.HasLogRepoReference())
                    .ToArray())
                {
                    foreach (var reference in csProj.LogRepoReferences())
                    {
                        csProj.UpdateLogRepoReference(reference);
                    }
                }
                if (slnFile.HasLogRepoReference())
                {
                    foreach (var reference in slnFile.LogRepoReferences())
                    {
                        slnFile.UpdateLogRepoReference(reference);
                    }
                }
            }
        }

        private static void PrintcsProjsDependantOnlogRepoSc(string dir)
        {
            foreach (var csproj in Dumbledore.GetProjectsDependantOnLogRepoSc(Dumbledore.GetCsProjFiles(dir)).ToArray())
            {
                Console.WriteLine(csproj.Path);
            }
        }

        public static string[] FilesToFix()
        {
            return new[]
            {
                @"c:\source\mercury\src\InvoiceService\DomainDeployer\DomainDeployer.csproj",
                @"c:\source\mercury\src\LegacyImport\AddressCacheDatabase\AddressCacheDatabase.csproj",
                @"c:\source\mercury\src\LegacyImport\TransitionDatabase\TransitionDatabase.csproj",
                @"c:\source\mercury\src\LetterDispatcherProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LetterDispatcherProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LettersReadModel\ReadModelDatabase\ReadModelDatabase.csproj",
                @"c:\source\mercury\src\LettersReadModel\ReadModelDeployer\ReadModelDeployer.csproj",
                @"c:\source\mercury\src\LettersReadModel\ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\LoanBankingDisbursementProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanBankingDisbursementProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanBankingDisbursementProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanBankingRepaymentProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanBankingRepaymentProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanBankingRepaymentProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanContractDocsProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanContractDocsProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanContractDocsProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanContractReadModel\ReadModelDatabase\ReadModelDatabase.csproj",
                @"c:\source\mercury\src\LoanContractReadModel\ReadModelDeployer\ReadModelDeployer.csproj",
                @"c:\source\mercury\src\LoanContractReadModel\ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\LoanContractService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LoanContractService\DomainDatabase\DomainDatabase.csproj",
                @"c:\source\mercury\src\LoanContractService\DomainDeployer\DomainDeployer.csproj",
                @"c:\source\mercury\src\LoanHardshipProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanHardshipProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanHardshipProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanNotice\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LoanNotice\DomainDatabase\DomainDatabase.csproj",
                @"c:\source\mercury\src\LoanNotice\DomainDeployer\DomainDeployer.csproj",
                @"c:\source\mercury\src\LoanNoticeProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanNoticeProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanNoticeProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanRatingProcess\ProcessDatabase\ProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanRatingProcess\ProcessDeployer\ProcessDeployer.csproj",
                @"c:\source\mercury\src\LoanRatingProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanRepaymentSchedulerProcess\LoanPaymentSchedulerProcessDatabase\LoanPaymentSchedulerProcessDatabase.csproj",
                @"c:\source\mercury\src\LoanRepaymentSchedulerProcess\LoanPaymentSchedulerProcessManager\LoanPaymentSchedulerProcessManager.csproj",
                @"c:\source\mercury\src\LoanService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LoanService\DomainDatabase\DomainDatabase.csproj",
                @"c:\source\mercury\src\LoanService\DomainDeployer\DomainDeployer.csproj",
                @"c:\source\mercury\src\LogRepository\LogQueueImporter\LogQueueImporter.csproj",
                @"c:\source\mercury\src\LogRepository\LogRepository.Deployer\LogRepository.Deployer.csproj",
                @"c:\source\mercury\src\LogRepository\LogRepository.LogRepositoryWebRole\WebRole.csproj",
                @"c:\source\mercury\src\Mediator\ApiDatabase\ApiDatabase.csproj",
                @"c:\source\mercury\src\Mediator\ApiDeployer\ApiDeployer.csproj",
                @"c:\source\mercury\src\Mediator\DomainDatabase\DomainDatabase.csproj",
                @"c:\source\mercury\src\Mediator\DomainDeployer\DomainDeployer.csproj",
                @"c:\source\mercury\src\Mediator\Mediator.MediatorCommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\Mediator\Mediator.MediatorWebRole\WebRole.csproj",
                @"c:\source\mercury\src\Mediator\Mediator.ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\MessageHub\MessageHub.Deployer\MessageHub.Deployer.csproj",
                @"c:\source\mercury\src\MessageHub\MessageHub.MessageHubDatabase\MessageHubDatabase.csproj",
                @"c:\source\mercury\src\MessageHub\MessageHub.MessageHubWebRole\WebRole.csproj"
            };
        }

        public static string[] FilesThatRequireNewRelic()
        {
            return new[]
            {
                @"c:\source\mercury\src\BPayPaymentDistributionProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\BankingProcessReadModel\ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\BankingService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\BatchPaymentProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\CashAdvanceRatingProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\CashAdvanceRatingService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\CollectionAgentBatchSubmissionProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\CollectionAgentService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\CollectionsImportExportProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\ContractSigningV2Process\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\CustomerCommunicationProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\CustomerCommunicationService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\CustomerNoteService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\CustomerService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\DisputeService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\DocumentService\ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\ESignService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\EmailDispatcherProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\HardshipService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\InStoreLoanApplicationProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\IncompleteApplication\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\InvoiceService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LetterDispatcherProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LettersReadModel\ReadModelPopulator\ReadModelPopulator.csproj",
                @"c:\source\mercury\src\LoanBankingDisbursementProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanBankingRepaymentProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanContractService\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LoanHardshipProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanNotice\CommandProcessor\CommandProcessor.csproj",
                @"c:\source\mercury\src\LoanNoticeProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanRatingProcess\ProcessManager\ProcessManager.csproj",
                @"c:\source\mercury\src\LoanRepaymentSchedulerProcess\LoanPaymentSchedulerProcessManager\LoanPaymentSchedulerProcessManager.csproj"
            };
        }
    }
}
