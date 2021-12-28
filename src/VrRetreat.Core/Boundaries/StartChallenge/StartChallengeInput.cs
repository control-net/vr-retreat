namespace VrRetreat.Core.Boundaries.StartChallenge;

public class StartChallengeInput
{
    public string Username { get; private set; }
    public string VrChatUsername { get; private set; }

    public StartChallengeInput(string username, string vrChatUsername)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        if (string.IsNullOrEmpty(vrChatUsername))
        {
            throw new ArgumentNullException(nameof(vrChatUsername));
        }

        Username = username;
        VrChatUsername = vrChatUsername;
    }
}
