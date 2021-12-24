namespace VrRetreat.Core.Boundaries.VrChatAccountClaim;

public interface IVrChatAccountClaimOutputPort
{
    void UnknownLoggedInUser();
    void UnknownVrChatUsername(string username);
    void UserHasCooldown();
    void SuccessfulClaim();
}
