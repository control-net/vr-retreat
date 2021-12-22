using Moq;
using Moq.Language.Flow;
using System;
using System.Threading.Tasks;
using VrRetreat.Core;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatAccountClaim;
using VrRetreat.Core.Entities;
using VrRetreat.Core.UseCases;
using VrRetreat.Infrastructure.Entities;
using Xunit;

namespace VrRetreat.Tests;

public class VrChatAccountClaimUseCaseTests
{
    private readonly IVrChatAccountClaimUseCase _sut;
    private readonly Mock<IVrChatAccountClaimOutputPort> _outputPortMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVrChat> _vrChatMock;
    private readonly Mock<IBioCodeGenerator> _bioCodeGeneratorMock;

    public VrChatAccountClaimUseCaseTests()
    {
        _outputPortMock = new Mock<IVrChatAccountClaimOutputPort>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vrChatMock = new Mock<IVrChat>();
        _bioCodeGeneratorMock = new Mock<IBioCodeGenerator>();
        _sut = new VrChatAccountClaimUseCase(_outputPortMock.Object, _userRepositoryMock.Object, _vrChatMock.Object, _bioCodeGeneratorMock.Object);
    }

    [Fact]
    public void NullUsername_ShouldThrow()
        => Assert.Throws<ArgumentNullException>(() => new VrChatAccountClaimInput(null!, "vrchatusername"));

    [Fact]
    public void NullOrEmptyVrChatUsername_ShouldThrow()
        => Assert.Throws<ArgumentNullException>(() => new VrChatAccountClaimInput("username", null!));

    [Fact]
    public async Task UnknownUsername_ShouldCallUnknownUserOutput()
    {
        ArrangeLoggedInUser(null!);

        var input = new VrChatAccountClaimInput("unknown-username", "vrchatusername");

        await _sut.ExecuteAsync(input);

        _outputPortMock.Verify(p => p.UnknownLoggedInUser(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UnknownVrChatUsername_ShouldCallUnknownVrChatUsernameOutput()
    {
        ArrangeLoggedInUser(new VrRetreatUser());
        ArrangeVrChatUser(null!);

        var vrcUsername = "unknown-vrchatusername";

        var input = new VrChatAccountClaimInput("username", vrcUsername);

        await _sut.ExecuteAsync(input);

        _outputPortMock.Verify(p => p.UnknownVrChatUsername(It.Is<string>(s => s == vrcUsername)), Times.Once);
    }

    [Fact]
    public async Task ValidInput_ShouldResetVrChatUserInfo()
    {
        var user = new VrRetreatUser
        {
            IsVrChatAccountLinked = true,
            VrChatName = "LinkedAccountName",
            VrChatAvatarUrl = "AvatarUrl",
            VrChatLastLogin = DateTime.Now,
            VrChatId = "Id"
        };
        ArrangeLoggedInUser(user);
        ArrangeVrChatUser(new());

        var input = new VrChatAccountClaimInput("username", "vrchatusername");

        await _sut.ExecuteAsync(input);

        VerifyUpdatedUser(u => 
            u.IsVrChatAccountLinked == false &&
            u.VrChatAvatarUrl == string.Empty &&
            u.VrChatLastLogin == null
        , "Did not properly reset all VRChat information.");
    }

    [Fact]
    public async Task ValidInput_ShouldSetupVrChatLinkValues()
    {
        var user = new VrRetreatUser
        {
            VrChatName = string.Empty,
            BioCode = "OldBioCode",
            LastUsernameCheck = DateTime.Now.AddHours(-1)
        };
        var vrcUser = new VrChatUser { Id = "VRChat ID" };
        ArrangeLoggedInUser(user);
        ArrangeVrChatUser(vrcUser);
        var vrcUsername = "vrchatusername";
        var bioCode = "12345";
        ArrangeBioCode(bioCode);

        var input = new VrChatAccountClaimInput("username", vrcUsername);

        await _sut.ExecuteAsync(input);

        VerifyUpdatedUser(u =>
            u.VrChatName == vrcUsername &&
            u.BioCode == bioCode &&
            u.VrChatId == vrcUser.Id
        , "Did not properly setup new VRChat link base.");
    }

    [Fact]
    public async Task RecentCheck_ShouldCallCooldownOutput()
    {
        var user = new VrRetreatUser
        {
            LastUsernameCheck = DateTime.Now.AddSeconds(-2)
        };
        ArrangeLoggedInUser(user);
        ArrangeVrChatUser(new());
        ArrangeBioCode("12345");

        var input = new VrChatAccountClaimInput("username", "vrchatusername");

        await _sut.ExecuteAsync(input);

        _outputPortMock.Verify(p => p.UserHasCooldown(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ValidInput_ShouldSetLastCallDateTime()
    {
        var user = new VrRetreatUser
        {
            LastUsernameCheck = null
        };
        ArrangeLoggedInUser(user);
        ArrangeVrChatUser(new());
        ArrangeBioCode("12345");

        var input = new VrChatAccountClaimInput("username", "vrchatusername");

        await _sut.ExecuteAsync(input);

        VerifyUpdatedUser(u => u.LastUsernameCheck is not null, "Did not set the LastUsernameCheck properly.");
    }

    private IReturnsResult<IBioCodeGenerator> ArrangeBioCode(string bioCode)
    {
        return _bioCodeGeneratorMock.Setup(g => g.GenerateNewCode()).Returns(bioCode);
    }

    [Fact]
    public async Task ValidInput_ShouldSendFriendRequest()
    {
        var vrcUser = new VrChatUser { Id = "VRChat ID" };
        ArrangeLoggedInUser(new VrRetreatUser());
        ArrangeVrChatUser(vrcUser);
        ArrangeBioCode("12345");

        var input = new VrChatAccountClaimInput("username", "vrchatusername");

        await _sut.ExecuteAsync(input);

        _vrChatMock.Verify(vrc => vrc.SendFriendRequestByUserId(It.Is<string>(id => id == vrcUser.Id)));
    }

    private void VerifyUpdatedUser(Func<VrRetreatUser, bool> condition, string message)
        => _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.Is<IVrRetreatUser>(u => condition((VrRetreatUser)u))), Times.Once, message);

    private void ArrangeVrChatUser(VrChatUser user)
        => _vrChatMock.Setup(vrc => vrc.GetPlayerByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

    private void ArrangeLoggedInUser(VrRetreatUser user)
        => _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
}
