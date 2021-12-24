namespace VrRetreat.Core.Boundaries.BioCodeVerification;

public class BioCodeVerificationInput
{
    public string Username { get; private set; }

    public BioCodeVerificationInput(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        Username = username;
    }
}
