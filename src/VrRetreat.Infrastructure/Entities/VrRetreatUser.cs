using Microsoft.AspNetCore.Identity;
using VrRetreat.Core.Entities;

namespace VrRetreat.Infrastructure.Entities;

public class VrRetreatUser : IdentityUser, IVrRetreatUser
{
    public string VrChatId { get; set; } = string.Empty;
    public string VrChatName { get; set; } = string.Empty;
    public string VrChatAvatarUrl { get; set; } = string.Empty;
    public DateTime? VrChatLastLogin { get; set; }
    public bool FailedChallenge { get; set; }
    public string BioCode { get; set; } = string.Empty;
    public bool IsVrChatAccountLinked { get; set; }
    public DateTime? LastFriendRequestSent { get; set; }
    public DateTime? LastBioCheck { get; set; }
    public DateTime? LastUsernameCheck { get; set; }

    public bool HasUsernameCheckCooldown => LastUsernameCheck is not null && (DateTime.Now - LastUsernameCheck).Value.TotalSeconds < 20;

    public bool HasFriendRequestCooldown => LastFriendRequestSent is not null && (DateTime.Now - LastFriendRequestSent).Value.TotalSeconds < 60;

    public bool HasBioRequestCooldown => LastBioCheck is not null && (DateTime.Now - LastBioCheck).Value.TotalSeconds < 60;

    public void ClearVrChatLink()
    {
        IsVrChatAccountLinked = false;
        VrChatAvatarUrl = string.Empty;
        VrChatId = string.Empty;
        VrChatLastLogin = null;
    }

    public void UpdateLastBioRequestCheck() => LastBioCheck = DateTime.Now;

    public void UpdateLastFriendRequestCheck() => LastFriendRequestSent = DateTime.Now;

    public void UpdateLastUsernameCheck() => LastUsernameCheck = DateTime.Now;
}
