namespace Content.Shared.CartridgeLoader.Cartridges.PdaRng;

/// <summary>
/// Rarity tiers derived from total badge XP.
/// Thresholds are tuned against the full 0..999999 distribution for the current badge catalog.
/// </summary>
public enum PdaRngTier
{
    Mundane,
    Notable,
    Irregular,
    Anomalous,
    Classified,
    RealityBend,
    Singularity,
}

public static class PdaRngTierHelper
{
    // Empirical XP cutoffs (~top 50 / 25 / 10 / 5 / 1 / 0.1%), nudged after Contains67 / ConsecutiveDigram.
    public const int NotableXp = 46;
    public const int IrregularXp = 73;
    public const int AnomalousXp = 98;
    public const int ClassifiedXp = 118;
    public const int RealityBendXp = 171;
    public const int SingularityXp = 275;

    /// <summary>
    /// Maps total badge XP to a tier and a display percentile value.
    /// Non-mundane tiers use a "top X%" framing; Mundane uses "bottom X%".
    /// </summary>
    public static (PdaRngTier Tier, float Percentile) FromXp(int totalXp)
    {
        return totalXp switch
        {
            >= SingularityXp => (PdaRngTier.Singularity, 0.1f),
            >= RealityBendXp => (PdaRngTier.RealityBend, 1f),
            >= ClassifiedXp => (PdaRngTier.Classified, 5f),
            >= AnomalousXp => (PdaRngTier.Anomalous, 10f),
            >= IrregularXp => (PdaRngTier.Irregular, 25f),
            >= NotableXp => (PdaRngTier.Notable, 50f),
            _ => (PdaRngTier.Mundane, EstimateMundaneBottomPercentile(totalXp)),
        };
    }

    private static float EstimateMundaneBottomPercentile(int totalXp)
    {
        // Lowest XP ≈ bottom ~99%; approaching Notable ≈ bottom ~50%.
        var span = Math.Max(1, NotableXp - 1);
        return Math.Clamp(99f - totalXp * (49f / span), 50f, 99f);
    }

    public static string GetLocId(PdaRngTier tier)
    {
        return tier switch
        {
            PdaRngTier.Mundane => "pda-rng-tier-mundane",
            PdaRngTier.Notable => "pda-rng-tier-notable",
            PdaRngTier.Irregular => "pda-rng-tier-irregular",
            PdaRngTier.Anomalous => "pda-rng-tier-anomalous",
            PdaRngTier.Classified => "pda-rng-tier-classified",
            PdaRngTier.RealityBend => "pda-rng-tier-reality-bend",
            PdaRngTier.Singularity => "pda-rng-tier-singularity",
            _ => "pda-rng-tier-mundane",
        };
    }

    /// <summary>Markup color used on printed tickets for the given tier.</summary>
    public static string GetColor(PdaRngTier tier)
    {
        return tier switch
        {
            PdaRngTier.Mundane => "#8a8a8a",
            PdaRngTier.Notable => "#4caf50",
            PdaRngTier.Irregular => "#2196f3",
            PdaRngTier.Anomalous => "#9c27b0",
            PdaRngTier.Classified => "#ff9800",
            PdaRngTier.RealityBend => "#f44336",
            PdaRngTier.Singularity => "#e91e63",
            _ => "#8a8a8a",
        };
    }
}
