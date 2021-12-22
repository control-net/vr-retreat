using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;

namespace VrRetreat.Core.UseCases;

public class VrChatAccountClaimUseCase : IVrChatAccountClaimUseCase
{
    private readonly IVrChatAccountClaimOutputPort _outputPort;
    private readonly IUserRepository _userRepository;
    private readonly IVrChat _vrChat;
    private readonly IBioCodeGenerator _bioCodeGenerator;

    public VrChatAccountClaimUseCase(IVrChatAccountClaimOutputPort outputPort, IUserRepository userRepository, IVrChat vrChat, IBioCodeGenerator bioCodeGenerator)
    {
        _outputPort = outputPort;
        _userRepository = userRepository;
        _vrChat = vrChat;
        _bioCodeGenerator = bioCodeGenerator;
    }

    public async Task ExecuteAsync(VrChatAccountClaimInput input)
    {
        var user = await _userRepository.GetUserByUsernameAsync(input.Username);

        if (user is null)
        {
            _outputPort.UnknownLoggedInUser();
            return;
        }

        if (user.HasUsernameCheckCooldown)
        {
            _outputPort.UserHasCooldown();
            return;
        }

        var vrcUser = await _vrChat.GetPlayerByNameAsync(input.VrChatUsername);
        user.UpdateLastUsernameCheck();

        if (vrcUser is null)
        {
            _outputPort.UnknownVrChatUsername(input.VrChatUsername);
            return;
        }

        user.ClearVrChatLink();
        user.VrChatName = input.VrChatUsername;
        user.BioCode = _bioCodeGenerator.GenerateNewCode();
        user.VrChatId = vrcUser.Id;

        await _userRepository.UpdateUserAsync(user);
        await _vrChat.SendFriendRequestByUserId(user.VrChatId);
    }
}
