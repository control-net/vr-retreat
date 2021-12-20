using VrRetreat.Core.Entities;

namespace VrRetreat.Core;

public interface IVrChat
{
    Task InitializeAsync();
    Task<bool> GetPlayerExistsAsync(string username);
    Task<VrChatUser> GetPlayerByNameAsync(string username);
    Task SendFriendRequestByUserId(string userId);
    Task<bool> IsFriendByUserId(string userId);
    Task<VrChatUser> GetPlayerByIdAsync(string userId);
}
