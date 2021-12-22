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
}
