using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.StartChallenge;

namespace VrRetreat.Core.UseCases;

public class StartChallengeUseCase : IStartChallengeUseCase
{
    private readonly IStartChallengeOutputPort _outputPort;
    private readonly IUserRepository _userRepository;
    private readonly IVrChat _vrChat;

    public StartChallengeUseCase(IStartChallengeOutputPort outputPort, IUserRepository userRepository, IVrChat vrChat)
    {
        _outputPort = outputPort;
        _userRepository = userRepository;
        _vrChat = vrChat;
    }

    public async Task ExecuteAsync(StartChallengeInput input)
    {
        var user = await _userRepository.GetUserByUsernameAsync(input.Username);

        if (user is null)
        {
            _outputPort.LoggedInUserNotFound();
            return;
        }

        if (user.FailedChallenge)
        {
            _outputPort.ChallengeFailed();
            return;
        }

        if (string.IsNullOrEmpty(user.VrChatId) || string.IsNullOrEmpty(user.VrChatName) || string.IsNullOrEmpty(user.BioCode))
        {
            _outputPort.NoClaimedVrChatAccount();
            return;
        }

        var vrcUser = await _vrChat.GetPlayerByNameAsync(user.VrChatName);
        user.UpdateLastUsernameCheck();

        if (vrcUser is null)
        {
            _outputPort.UnknownVrChatUsername(user.VrChatName);
            return;
        }

        user.VrChatLastLogin = vrcUser.LastLogin;
        user.IsParticipating = true;

        await _userRepository.UpdateUserAsync(user);
        _outputPort.SuccessfulStart();
    }
}

