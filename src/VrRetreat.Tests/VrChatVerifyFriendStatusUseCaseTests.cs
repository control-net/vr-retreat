using Moq;
using System;
using System.Threading.Tasks;
using VrRetreat.Core;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;
using VrRetreat.Core.Entities;
using VrRetreat.Core.UseCases;
using VrRetreat.Infrastructure.Entities;
using Xunit;

namespace VrRetreat.Tests;

public class VrChatVerifyFriendStatusUseCaseTests
{
    private readonly IVrChatVerifyFriendStatusUseCase _sut;
    private readonly Mock<IVrChatVerifyFriendStatusOutputPort> _outputPortMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVrChat> _vrChatMock;

    public VrChatVerifyFriendStatusUseCaseTests()
    {
        _outputPortMock = new Mock<IVrChatVerifyFriendStatusOutputPort>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vrChatMock = new Mock<IVrChat>();

        _sut = new VrChatVerifyFriendStatusUseCase(_outputPortMock.Object, _userRepositoryMock.Object, _vrChatMock.Object);
    }

    [Fact]
    public async Task InvalidLoggedInUser_ShouldOutputCorrectly()
    {
        ArrangeLoggedInUser(null!);

        await _sut.ExecuteAsync(new("username"));

        _outputPortMock.Verify(p => p.LoggedInUserNotFound(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NoClaimedVrChatAccount_ShouldOutputCorrectly()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = string.Empty,
            VrChatName = string.Empty
        });

        await _sut.ExecuteAsync(new("username"));

        _outputPortMock.Verify(p => p.NoClaimedVrChatAccount(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UserWithCooldown_ShouldOutputCorrectly()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = "id",
            VrChatName = "name",
            LastFriendRequestSent = DateTime.Now
        });

        await _sut.ExecuteAsync(new("username"));

        _outputPortMock.Verify(p => p.UserHasCooldown(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UserNotFriended_ShouldResendRequest_And_Output()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = "id",
            VrChatName = "name"
        });
        _vrChatMock.Setup(vrc => vrc.IsFriendByUserId(It.IsAny<string>())).ReturnsAsync(false);

        await _sut.ExecuteAsync(new("username"));

        _vrChatMock.Verify(vrc => vrc.SendFriendRequestByUserId(It.IsAny<string>()), Times.Once);
        _outputPortMock.Verify(p => p.UserNotFriended(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Valid_ShouldSetCooldown_And_Output()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = "id",
            VrChatName = "name",
            LastFriendRequestSent = null
        });
        _vrChatMock.Setup(vrc => vrc.IsFriendByUserId(It.IsAny<string>())).ReturnsAsync(true);

        await _sut.ExecuteAsync(new("username"));

        VerifyUpdatedUser(u => u.LastFriendRequestSent is not null, "Didn't set cooldown property properly.");
        _outputPortMock.Verify(p => p.FriendshipVerified(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    private void VerifyUpdatedUser(Func<VrRetreatUser, bool> condition, string message)
    => _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.Is<IVrRetreatUser>(u => condition((VrRetreatUser)u))), Times.Once, message);

    private void ArrangeVrChatUser(VrChatUser user)
    => _vrChatMock.Setup(vrc => vrc.GetPlayerByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

    private void ArrangeLoggedInUser(VrRetreatUser user)
        => _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
}
