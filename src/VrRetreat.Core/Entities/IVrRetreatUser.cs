namespace VrRetreat.Core.Entities;

public interface IVrRetreatUser
{
    string VrChatId { get; set; }
    string VrChatName { get; set; }
    string BioCode { get; set; }
    string VrChatAvatarUrl { get; set; }
    DateTime? VrChatLastLogin { get; set; }
    bool FailedChallenge { get; set; }
    bool IsVrChatAccountLinked { get; set; }
    bool HasUsernameCheckCooldown { get; }
    void UpdateLastUsernameCheck();
    void ClearVrChatLink();
    bool HasFriendRequestCooldown { get; }
    void UpdateLastFriendRequestCheck();
    bool HasBioRequestCooldown { get; }
    void UpdateLastBioRequestCheck();
    bool IsParticipating { get; set; }
}
