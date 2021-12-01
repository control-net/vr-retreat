using VRChat.API.Api;
using VRChat.API.Client;
using VrRetreat.Core;

namespace VrRetreat.Infrastructure
{
    public class VrChat : IVrChat
    {
        private readonly AuthenticationApi _auth;

        public VrChat(IConfiguration config)
        {
            _auth = new AuthenticationApi(new Configuration
            {
                Username = config.VrChatUsername,
                Password = config.VrChatPassword
            });
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
    }
}
