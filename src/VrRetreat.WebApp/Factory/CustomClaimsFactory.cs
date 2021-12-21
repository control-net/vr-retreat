using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using VrRetreat.Infrastructure.Entities;

namespace VrRetreat.WebApp.Factory;

public class CustomClaimsFactory : UserClaimsPrincipalFactory<VrRetreatUser>
{
    public CustomClaimsFactory(UserManager<VrRetreatUser> userManager, IOptions<IdentityOptions> optionsAccessor)
    : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(VrRetreatUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim("avatarurl", string.IsNullOrWhiteSpace(user.VrChatAvatarUrl) ? "/img/anon.webp" : user.VrChatAvatarUrl));

        return identity;
    }
}
