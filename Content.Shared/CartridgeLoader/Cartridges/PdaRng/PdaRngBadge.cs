namespace Content.Shared.CartridgeLoader.Cartridges.PdaRng;

/// <summary>
/// A single badge that a rolled number may earn.
/// <see cref="Id"/> maps to locale keys <c>pda-rng-badge-{Id}</c> (name)
/// and <c>pda-rng-badge-{Id}.desc</c> (description).
/// </summary>
public sealed class PdaRngBadge
{
    public PdaRngBadge(string id, int xp, Func<int, string, bool> matches)
    {
        Id = id;
        Xp = xp;
        Matches = matches;
    }

    /// <summary>Stable id used for localization and review.</summary>
    public string Id { get; }

    /// <summary>Entropy points awarded when this badge is earned.</summary>
    public int Xp { get; }

    /// <summary>
    /// Predicate over the rolled value and its natural decimal string (no leading zeros, except "0").
    /// </summary>
    public Func<int, string, bool> Matches { get; }
}

/// <summary>A badge earned by a specific roll, ready for ticket printing.</summary>
public readonly record struct PdaRngEarnedBadge(string Id, int Xp);
