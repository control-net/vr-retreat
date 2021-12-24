namespace VrRetreat.Core.Boundaries.VrChatVerifyFriendStatus;

public interface IVrChatVerifyFriendStatusUseCase
{
    Task ExecuteAsync(VrChatVerifyFriendStatusInput input);
}
