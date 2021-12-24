using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Boundaries.Infrastructure;

namespace VrRetreat.Core.UseCases;

public class BioCodeVerificationUseCase : IBioCodeVerificationUseCase
{
    private readonly IBioCodeVerificationOutputPort _outputPort;
    private readonly IUserRepository _userRepository;
    private readonly IVrChat _vrChat;

    public BioCodeVerificationUseCase(IBioCodeVerificationOutputPort outputPort, IUserRepository userRepository, IVrChat vrChat)
    {
        _outputPort = outputPort;
        _userRepository = userRepository;
        _vrChat = vrChat;
    }

    public async Task ExecuteAsync(BioCodeVerificationInput input)
    {
        var user = await _userRepository.GetUserByUsernameAsync(input.Username);

        if (user is null)
        {
            _outputPort.LoggedInUserNotFound();
            return;
        }

        if (string.IsNullOrEmpty(user.VrChatId) || string.IsNullOrEmpty(user.VrChatName) || string.IsNullOrEmpty(user.BioCode))
        {
            _outputPort.NoClaimedVrChatAccount();
            return;
        }

        if (user.HasBioRequestCooldown)
        {
            _outputPort.UserHasCooldown();
            return;
        }

        var vrcUser = await _vrChat.GetPlayerByIdAsync(user.VrChatId);
        user.UpdateLastBioRequestCheck();

        if (vrcUser is null)
        {
            await _userRepository.UpdateUserAsync(user);
            _outputPort.NoClaimedVrChatAccount();
            return;
        }

        if (!vrcUser.Bio.Contains(user.BioCode))
        {
            await _userRepository.UpdateUserAsync(user);
            _outputPort.BioCodeNotFound();
            return;
        }

        user.VrChatLastLogin = vrcUser.LastLogin;
        user.VrChatAvatarUrl = vrcUser.AvatarUrl;
        user.IsVrChatAccountLinked = true;
        user.FailedChallenge = false;

        await _userRepository.UpdateUserAsync(user);

        _outputPort.BioCodeVerified();
    }
}
