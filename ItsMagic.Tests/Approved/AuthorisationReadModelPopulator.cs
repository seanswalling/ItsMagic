using System;
using Autofac;
using Mercury.Core.AutofacScanning;
using Mercury.Plugins.AuthorisationReadModel.Providers;
using Mercury.Tests.Core.Scenarios;
using Mercury.WorkerEngine.Infrastructure;
using WorkerEngine.TestCommon;

namespace Mercury.Plugins.AuthorisationReadModel.Tests
{
    public class AuthorisationReadModelPopulator : ScenarioDrivenTestService
    {
        public AuthorisationReadModelPopulator()
            : base(WorkerStyle.ReadModelPopulator)
        {
        }

        protected override void AdditionalRegistration(ContainerBuilder containerBuilder)
        {
            base.AdditionalRegistration(containerBuilder);

            containerBuilder.ScanAssemblyContaining<CurrentUserProvider>();
        }
    }
}
