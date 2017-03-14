using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            string dir = "C:\\source\\Mercury\\src";
            //string dir = @"E:\github\cc\Mercury\src";
            //Dumbledore.AddJExtAndNHibExtReferences(dir);
            //Dumbledore.RemoveLogForNetReference(FilesToFix());
            Dumbledore.FixNHibExtUsings(Directory.EnumerateFiles(dir,"*.cs",SearchOption.AllDirectories).ToArray());
            Console.WriteLine("Application Complete");
            Console.ReadLine();
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
    }
}
