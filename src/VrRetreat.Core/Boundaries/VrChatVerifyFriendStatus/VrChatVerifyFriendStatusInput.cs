namespace VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;

public class VrChatVerifyFriendStatusInput
{
    public string Username { get; private set; }

    public VrChatVerifyFriendStatusInput(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        Username = username;
    }
}
