using Content.Shared.CartridgeLoader.Cartridges.PdaRng;

var counts = new int[Enum.GetValues<PdaRngTier>().Length];
var xpList = new List<int>(1_000_000);
long xpSum = 0;

for (var n = 0; n <= 999_999; n++)
{
    var result = PdaRngAnalyzer.Analyze(n);
    counts[(int)result.Tier]++;
    xpList.Add(result.TotalXp);
    xpSum += result.TotalXp;
}

xpList.Sort();
var total = xpList.Count;
Console.WriteLine($"mean={xpSum / (double)total:F2} median={xpList[total / 2]}");
Console.WriteLine("Tier shares after Option B:");
foreach (PdaRngTier tier in Enum.GetValues<PdaRngTier>())
    Console.WriteLine($"  {tier,-12} {counts[(int)tier],8:N0}  ({counts[(int)tier] * 100.0 / total,6:F2}%)");

Console.WriteLine();
Console.WriteLine("Example boring / spicy rolls:");
foreach (var n in new[] { 703481, 482917, 250000, 123456, 69, 1, 555555 })
{
    var r = PdaRngAnalyzer.Analyze(n);
    Console.WriteLine($"  {n,6} XP={r.TotalXp,3} Tier={r.Tier,-12} pct={r.Percentile} badges={r.Badges.Count}");
}
