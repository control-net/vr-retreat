namespace VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;

public interface IVrChatVerifyFriendStatusOutputPort
{
    void LoggedInUserNotFound();
    void NoClaimedVrChatAccount();
    void UserHasCooldown();
    void UserNotFriended();
    void FriendshipVerified();
}
