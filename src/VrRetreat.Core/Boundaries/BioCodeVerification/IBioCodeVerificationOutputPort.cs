namespace VrRetreat.Core.Boundaries.BioCodeVerification;

public interface IBioCodeVerificationOutputPort
{
    void LoggedInUserNotFound();
    void NoClaimedVrChatAccount();
    void UserHasCooldown();
    void BioCodeNotFound();
    void BioCodeVerified();
}
