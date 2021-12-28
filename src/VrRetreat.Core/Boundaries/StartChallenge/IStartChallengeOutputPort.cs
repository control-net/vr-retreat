namespace VrRetreat.Core.Boundaries.StartChallenge;

public interface IStartChallengeOutputPort
{
    void LoggedInUserNotFound();
    void NoClaimedVrChatAccount();
    void RedirectToIndex();
    void ChallengeFailed();
    void SuccessfulStart();

}

