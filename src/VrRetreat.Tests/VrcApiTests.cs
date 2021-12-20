using System.Threading.Tasks;
using VrRetreat.Core;
using VrRetreat.Infrastructure;
using Xunit;

namespace VrRetreat.Tests;

public class VrcApiTests
{
    private readonly IVrChat _sut;

    public VrcApiTests()
    {
        _sut = new VrChat(JsonConfiguration.FromFile("TestConfig.json"));
    }

    [Theory(Skip = "Officially, we're not supposed to call the API more than once per 60 seconds!")]
    [InlineData("Spelos", true)]
    [InlineData("7cb3c694-43df-4923-9a2e-11f6c316e206", false)]
    public async Task ExistingUser_ShouldReturnExists(string username, bool expected)
    {
        await _sut.InitializeAsync();

        var actual = await _sut.GetPlayerExistsAsync(username);

        Assert.Equal(expected, actual);
    }

    [Fact(Skip = "Officially, we're not supposed to call the API more than once per 60 seconds!")]
    public async Task GetUserByName_ShouldReturnCorrectUser()
    {
        await _sut.InitializeAsync();

        var actual = await _sut.GetPlayerByNameAsync("Timmy");

        Assert.NotNull(actual);
    }

    [Fact(Skip = "Officially, we're not supposed to call the API more than once per 60 seconds!")]
    public async Task GetUserById_ShouldReturnCorrectUser()
    {
        await _sut.InitializeAsync();

        var actual = await _sut.GetPlayerByIdAsync("usr_d002d587-a0d8-4d3c-ab22-1e42325c0cd8");

        Assert.NotNull(actual);
    }

    [Fact(Skip = "Officially, we're not supposed to call the API more than once per 60 seconds!")]
    public async Task SendFriendRequest_ShouldNotThrow()
    {
        await _sut.InitializeAsync();

        await _sut.SendFriendRequestByUserId("usr_d002d587-a0d8-4d3c-ab22-1e42325c0cd8");
    }

    [Fact(Skip = "Officially, we're not supposed to call the API more than once per 60 seconds!")]
    public async Task IsFriend_ShouldReturnCorrectly()
    {
        await _sut.InitializeAsync();

        var isSpelosFriend = await _sut.IsFriendByUserId("usr_d002d587-a0d8-4d3c-ab22-1e42325c0cd8");
        var isTimmyFriend = await _sut.IsFriendByUserId("usr_556f90e8-122b-4267-bb54-026dd1f7d0cb");

        Assert.True(isSpelosFriend);
        Assert.False(isTimmyFriend);
    }
}
