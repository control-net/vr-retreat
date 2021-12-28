using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Boundaries.StartChallenge;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;
using VrRetreat.WebApp.Presenters;

namespace VrRetreat.WebApp.Extensions
{
    public static class UserInterfaceExtensions
    {
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            services.AddScoped<VrChatAccountClaimPresenter>();
            services.AddScoped<IVrChatAccountClaimOutputPort, VrChatAccountClaimPresenter>(x => x.GetRequiredService<VrChatAccountClaimPresenter>());

            services.AddScoped<VrChatVerifyFriendStatusPresenter>();
            services.AddScoped<IVrChatVerifyFriendStatusOutputPort, VrChatVerifyFriendStatusPresenter>(x => x.GetRequiredService<VrChatVerifyFriendStatusPresenter>());

            services.AddScoped<BioCodeVerificationPresenter>();
            services.AddScoped<IBioCodeVerificationOutputPort, BioCodeVerificationPresenter>(x => x.GetRequiredService<BioCodeVerificationPresenter>());

            services.AddScoped<StartChallengePresenter>();
            services.AddScoped<IStartChallengeOutputPort, StartChallengePresenter>(x => x.GetRequiredService<StartChallengePresenter>());

            return services;
        }
    }
}
