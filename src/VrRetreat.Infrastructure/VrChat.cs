using VRChat.API.Api;
using VRChat.API.Client;
using VrRetreat.Core;
using VrRetreat.Core.Entities;

namespace VrRetreat.Infrastructure;

public class VrChat : IVrChat
{
    private readonly AuthenticationApi _auth;
    private readonly UsersApi _users;

    public VrChat(IConfiguration config)
    {
        Configuration vrConfig = new Configuration
        {
            Username = config.VrChatUsername,
            Password = config.VrChatPassword
        };

        _auth = new AuthenticationApi(vrConfig);
        _users = new UsersApi(vrConfig);
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

    public async Task GetPlayerByNameAsync(string username)
    {
        var result = await _users.GetUserByNameAsync(username);
    }
}
