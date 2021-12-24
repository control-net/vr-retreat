using Moq;
using System;
using System.Threading.Tasks;
using VrRetreat.Core;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.BioCodeVerification;
using VrRetreat.Core.Entities;
using VrRetreat.Core.UseCases;
using VrRetreat.Infrastructure.Entities;
using Xunit;

namespace VrRetreat.Tests;

public class BioCodeVerificationUseCaseTests
{
    private readonly IBioCodeVerificationUseCase _sut;
    private readonly Mock<IBioCodeVerificationOutputPort> _outputPortMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVrChat> _vrChatMock;

    public BioCodeVerificationUseCaseTests()
    {
        _outputPortMock = new Mock<IBioCodeVerificationOutputPort>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vrChatMock = new Mock<IVrChat>();

        _sut = new BioCodeVerificationUseCase(_outputPortMock.Object, _userRepositoryMock.Object, _vrChatMock.Object);
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
            BioCode = "12345",
            LastBioCheck = DateTime.Now
        });

        await _sut.ExecuteAsync(new("username"));

        _outputPortMock.Verify(p => p.UserHasCooldown(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BioCodeNotFound_ShouldOutputCorrectly()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = "id",
            VrChatName = "name",
            BioCode = "12345",
            LastBioCheck = null
        });
        ArrangeVrChatUser(new()
        {
            Bio = "bio without code"
        });

        await _sut.ExecuteAsync(new("username"));

        VerifyUpdatedUser(u => u.LastBioCheck is not null, "Didn't set cooldown property properly.");
        _outputPortMock.Verify(p => p.BioCodeNotFound(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Valid_ShouldSetCooldown_And_Output()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = "id",
            VrChatName = "name",
            BioCode = "12345",
            LastBioCheck = null
        });
        ArrangeVrChatUser(new()
        {
            Bio = "some bio \nwith12345 code in it"
        });

        await _sut.ExecuteAsync(new("username"));

        VerifyUpdatedUser(u => u.LastBioCheck is not null, "Didn't set cooldown property properly.");
        _outputPortMock.Verify(p => p.BioCodeVerified(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    private void VerifyUpdatedUser(Func<VrRetreatUser, bool> condition, string message)
    => _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.Is<IVrRetreatUser>(u => condition((VrRetreatUser)u))), Times.Once, message);

    private void ArrangeVrChatUser(VrChatUser user)
    => _vrChatMock.Setup(vrc => vrc.GetPlayerByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

    private void ArrangeLoggedInUser(VrRetreatUser user)
        => _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
}
