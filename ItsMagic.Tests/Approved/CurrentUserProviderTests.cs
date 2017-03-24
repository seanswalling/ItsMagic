using System;
using System.Security.Principal;
using System.Threading;
using Mercury.Contracts.Permissions;
using Mercury.Contracts.Tests.Scenarios;
using Mercury.Core;
using Mercury.Plugins.AuthorisationReadModel.Providers;
using Mercury.Tests.Core;
using Newtonsoft.Json;
using Xunit;

namespace Mercury.Plugins.AuthorisationReadModel.Tests
{
    public class CurrentUserProviderTests : AuthorisationReadModelPopulator
    {
        [Fact]
        public void ReturnsUserSummaryForCurrentPrincipal()
        {
            var userId = Guid.NewGuid();
            var username = Randomness.Instance.Username();
            var roles = Randomness.Instance.PickSome(EnumEx.GetValues<MercuryRole>());
            var permissions = Randomness.Instance.PickSome(EnumEx.GetValues<MercuryPermission>());

            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), new string[0]);

            Given(UserScenarions.AUserIsCreated(userId, username, roles, permissions));

            var result = Resolve<ICurrentUserProvider>().GetCurrentUser();

            result.ShouldBeLike(new
            {
                UserId = userId,
                Username = username,
                Roles = JsonConvert.SerializeObject(roles),
                Permissions = JsonConvert.SerializeObject(permissions)
            });
        }
    }
}
