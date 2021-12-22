using VrRetreat.Core.Boundaries.Infrastructure;

namespace VrRetreat.Infrastructure;

public class BioCodeGenerator : IBioCodeGenerator
{
    const int NumberOfDigits = 5;

    private readonly Random _random;

    public BioCodeGenerator(Random random)
    {
        _random = random;
    }

    public string GenerateNewCode()
        => string.Join(string.Empty, Enumerable.Range(0, NumberOfDigits).Select(n => _random.Next(0, 10)));
}
