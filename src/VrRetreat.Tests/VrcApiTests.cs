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
    private async Task GetUserByName_ShouldReturnCorrectUser()
    {
        await _sut.InitializeAsync();

        //var actual = await _sut.GetPlayerByNameAsync("Alanthea");

        //Assert.NotNull(actual);
    }
}
