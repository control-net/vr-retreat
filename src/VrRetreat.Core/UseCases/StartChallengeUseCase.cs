using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.StartChallenge;

namespace VrRetreat.Core.UseCases;

public class StartChallengeUseCase : IStartChallengeUseCase
{
    private readonly IStartChallengeOutputPort _outputPort;
    private readonly IUserRepository _userRepository;

    public StartChallengeUseCase(IStartChallengeOutputPort outputPort, IUserRepository userRepository)
    {
        _outputPort = outputPort;
        _userRepository = userRepository;
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

        user.VrChatLastLogin = DateTime.Now;
        user.IsParticipating = true;

        await _userRepository.UpdateUserAsync(user);
        _outputPort.SuccessfulStart();
    }
}

