namespace VrRetreat.Core.Boundaries.StartChallenge;

public class StartChallengeInput
{
    public string Username { get; private set; }

    public StartChallengeInput(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        Username = username;
    }
}
