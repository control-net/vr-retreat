using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;

namespace VrRetreat.Core.UseCases;

public class VrChatVerifyFriendStatusUseCase : IVrChatVerifyFriendStatusUseCase
{
    private readonly IVrChatVerifyFriendStatusOutputPort _outputPort;
    private readonly IUserRepository _userRepository;
    private readonly IVrChat _vrChat;

    public VrChatVerifyFriendStatusUseCase(IVrChatVerifyFriendStatusOutputPort outputPort, IUserRepository userRepository, IVrChat vrChat)
    {
        this._outputPort = outputPort;
        this._userRepository = userRepository;
        this._vrChat = vrChat;
    }

    public async Task ExecuteAsync(VrChatVerifyFriendStatusInput input)
    {
        var user = await _userRepository.GetUserByUsernameAsync(input.Username);

        if (user is null)
        {
            _outputPort.LoggedInUserNotFound();
            return;
        }

        if (string.IsNullOrEmpty(user.VrChatName) || string.IsNullOrEmpty(user.VrChatId))
        {
            _outputPort.NoClaimedVrChatAccount();
            return;
        }

        if (user.HasFriendRequestCooldown)
        {
            _outputPort.UserHasCooldown();
            return;
        }

        if (await _vrChat.IsFriendByUserId(user.VrChatId) == false)
        {
            await _vrChat.SendFriendRequestByUserId(user.VrChatId);
            user.UpdateLastFriendRequestCheck();
            await _userRepository.UpdateUserAsync(user);
            _outputPort.UserNotFriended();
            return;
        }

        user.UpdateLastFriendRequestCheck();
        await _userRepository.UpdateUserAsync(user);
        _outputPort.FriendshipVerified();
    }
}
