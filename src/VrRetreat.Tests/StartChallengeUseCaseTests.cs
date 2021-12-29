using Moq;
using System;
using System.Threading.Tasks;
using VrRetreat.Core;
using VrRetreat.Core.Boundaries.Infrastructure;
using VrRetreat.Core.Boundaries.StartChallenge;
using VrRetreat.Core.Entities;
using VrRetreat.Core.UseCases;
using VrRetreat.Infrastructure.Entities;
using Xunit;

namespace VrRetreat.Tests;

public class StartChallengeUseCaseTests
{
    private readonly IStartChallengeUseCase _sut;
    private readonly Mock<IStartChallengeOutputPort> _outputPortMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IVrChat> _vrChatMock;

    public StartChallengeUseCaseTests()
    {
        _outputPortMock = new Mock<IStartChallengeOutputPort>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _vrChatMock = new Mock<IVrChat>();
        _sut = new StartChallengeUseCase(_outputPortMock.Object, _userRepositoryMock.Object, _vrChatMock.Object);
    }

    [Fact]
    public void NullUsername_ShouldThrow()
    => Assert.Throws<ArgumentNullException>(() => new StartChallengeInput(null!));

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
    public async Task ChallangeFailed_ShouldOutputCorreclty()
    {
        ArrangeLoggedInUser(new()
        {
            VrChatId = string.Empty,
            VrChatName = string.Empty,
            FailedChallenge = true
        });

        await _sut.ExecuteAsync(new("username"));

        _outputPortMock.Verify(p => p.ChallengeFailed(), Times.Once);
        _outputPortMock.VerifyNoOtherCalls();
    }

    private void ArrangeLoggedInUser(VrRetreatUser user)
       => _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
}
