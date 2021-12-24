namespace VrRetreat.Core.Boundaries.BioCodeVerification;

public interface IBioCodeVerificationUseCase
{
    Task ExecuteAsync(BioCodeVerificationInput input);
}
