namespace VrRetreat.Core.Boundaries.VrChatAccountClaim;

public interface IVrChatAccountClaimUseCase
{
    Task ExecuteAsync(VrChatAccountClaimInput input);
}
