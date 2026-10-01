namespace Content.Shared.CartridgeLoader.Cartridges.PdaRng;

/// <summary>Fully analyzed result of a PDA RNG roll.</summary>
public sealed class PdaRngRollResult
{
    public required int Number { get; init; }
    public required string NumberText { get; init; }
    public required IReadOnlyList<PdaRngEarnedBadge> Badges { get; init; }
    public required int TotalXp { get; init; }
    public required PdaRngTier Tier { get; init; }

    /// <summary>
    /// For Mundane: bottom percentile. For all other tiers: top percentile.
    /// </summary>
    public required float Percentile { get; init; }

    public required int DigitCount { get; init; }
    public required int DigitSum { get; init; }
    public required int UniqueDigits { get; init; }
    public required int OddDigitCount { get; init; }
    public required int EvenDigitCount { get; init; }
}
