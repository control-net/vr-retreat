using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;
using VrRetreat.Core.UseCases;

namespace VrRetreat.WebApp.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<IVrChatAccountClaimUseCase, VrChatAccountClaimUseCase>();
            services.AddScoped<IVrChatVerifyFriendStatusUseCase, VrChatVerifyFriendStatusUseCase>();
            services.AddScoped<IBioCodeVerificationUseCase, BioCodeVerificationUseCase>();

            return services;
        }
    }
}
