namespace VrRetreat.Core.Entities;

public interface IVrRetreatUser
{
    string VrChatId { get; set; }
    string VrChatName { get; set; }
    string BioCode { get; set; }
    bool HasUsernameCheckCooldown { get; }
    void UpdateLastUsernameCheck();
    void ClearVrChatLink();
}
