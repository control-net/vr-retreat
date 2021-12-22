using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VrRetreat.Core;
using VrRetreat.Core.Entities;

namespace VrRetreat.Infrastructure;

public class VrChat : IVrChat
{
    private readonly AuthenticationApi _auth;
    private readonly UsersApi _users;
    private readonly FriendsApi _friends;
    private readonly NotificationsApi _notifications;

    public VrChat(IConfiguration config)
    {
        Configuration vrConfig = new()
        {
            Username = config.VrChatUsername,
            Password = config.VrChatPassword
        };

        _auth = new AuthenticationApi(vrConfig);
        _users = new UsersApi(vrConfig);
        _friends = new FriendsApi(vrConfig);
        _notifications = new NotificationsApi(vrConfig);
    }

    public async Task InitializeAsync()
    {
        // NOTE(Peter): Getting the current user
        //              sets the required API Key.
        _ = await _auth.GetCurrentUserAsync();
    }

    public async Task<bool> GetPlayerExistsAsync(string username)
    {
        var result = await _auth.CheckUserExistsAsync(displayName: username);
        return result._UserExists;
    }

    public async Task<VrChatUser> GetPlayerByNameAsync(string username)
    {
        try
        {
            var result = await _users.GetUserByNameAsync(username);
            return ToVrChatUser(result);
        }
        catch (ApiException e)
        {
            if (e.ErrorCode == 404)
                return null!;

            throw;
        }
    }

    private VrChatUser ToVrChatUser(User result) => new()
    {
        AvatarUrl = GetAvatarUrl(result),
        Bio = result.Bio,
        Id = result.Id,
        Name = result.DisplayName,
        LastLogin = GetLastLogin(result)
    };

    private DateTime? GetLastLogin(User result)
        => string.IsNullOrWhiteSpace(result.LastLogin) ? null : DateTime.Parse(result.LastLogin);

    private static string GetAvatarUrl(User result)
        => string.IsNullOrWhiteSpace(result.ProfilePicOverride) ? result.CurrentAvatarImageUrl : result.ProfilePicOverride;

    public async Task SendFriendRequestByUserId(string userId)
    {
        var status = await _friends.GetFriendStatusAsync(userId);

        if (status.IsFriend)
            return;

        if (status.IncomingRequest)
        {
            await _notifications.AcceptFriendRequestAsync(userId);
            return;
        }

        if (status.OutgoingRequest)
        {
            await _friends.DeleteFriendRequestAsync(userId);
        }

        _ = await _friends.FriendAsync(userId);
    }

    public async Task<bool> IsFriendByUserId(string userId)
    {
        var status = await _friends.GetFriendStatusAsync(userId);

        return status.IsFriend;
    }

    public async Task<VrChatUser> GetPlayerByIdAsync(string userId)
    {
        var result = await _users.GetUserAsync(userId);

        return ToVrChatUser(result);
    }
}
