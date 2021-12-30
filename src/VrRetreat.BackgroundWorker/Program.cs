using Microsoft.EntityFrameworkCore;
using System.Text;
using VrRetreat.Infrastructure;
using VrRetreat.Infrastructure.Entities;

var logBuilder = new StringBuilder();
void Log(string message) => logBuilder.AppendLine($"{DateTime.Now:G} - {message}");

DbContextOptions<ApplicationDbContext> options = new();

var context = new ApplicationDbContext(options, "connString.txt");

bool ShouldUserBeChecked(VrRetreatUser user)
    => user is not null && user.IsParticipating && !user.FailedChallenge;

var vrc = new VrChat(JsonConfiguration.FromFile("VrChatConfig.json"));
var usersToCheck = context.Users.Where(ShouldUserBeChecked).ToList();
var challengeStart = new DateOnly(2022, 1, 1);
var logoutRequired = false;

foreach (var user in usersToCheck)
{
    await Task.Delay(TimeSpan.FromMinutes(1));
    Log($"Processing {user.UserName}");

    var vrcUser = await vrc.GetPlayerByIdAsync(user.VrChatId);
    logoutRequired = true;

    if (vrcUser is null)
    {
        //NOTE(Peter): If we fail to fetch a user from vrc
        //             then something larger than us failed
        //             let's ignore it for now.
        Log("Couldn't fetch VRChat account information. Skipping...");
        continue;
    }

    if (user.VrChatLastLogin is null)
    {
        Log("The user doesn't have a last known login time.");

        if (vrcUser.LastLogin is null)
        {
            Log("VRChat was unable to provide a last login time. Forced to skip...");
            continue;
        }

        Log("Updating last login time...");
        user.VrChatLastLogin = vrcUser.LastLogin;
        continue;
    }

    if (vrcUser.LastLogin is null)
    {
        Log("LastLogin is null, the user most likely unfriended the bot. Failing the challenge...");

        user.FailedChallenge = true;
        continue;
    }

    var vrcLastLogin = vrcUser.LastLogin.Value;
    var knownLastLogin = user.VrChatLastLogin.Value;

    if (DateOnly.FromDateTime(vrcLastLogin) <= challengeStart)
    {
        Log("The last login is within the challenge's grace period. Applying last login.");

        if (vrcLastLogin != knownLastLogin)
            user.VrChatLastLogin = vrcLastLogin;

        continue;
    }

    if (DateOnly.FromDateTime(vrcLastLogin) > DateOnly.FromDateTime(knownLastLogin))
    {
        Log($"The user failed the challenge because {vrcLastLogin}(as a day) > {knownLastLogin}(as a day)");
        user.FailedChallenge = true;
    }
    else
    {
        Log($"The user is still in the challenge because {vrcLastLogin}(as a day) <= {knownLastLogin}(as a day)");
    }
}

await context.SaveChangesAsync();

if(logoutRequired)
    await vrc.LogoutAsync();

await context.DisposeAsync();
File.WriteAllText("bgworker.log", logBuilder.ToString());
