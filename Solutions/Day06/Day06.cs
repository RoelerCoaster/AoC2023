using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day06;

internal class Day06 : DayBase
{
    public override int Day => 6;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var lines = input.Lines();

        var durations = lines[0].Replace("Time:", "").Trim().NumbersByRegex<long>("\\s+");
        var records = lines[1].Replace("Distance:", "").Trim().NumbersByRegex<long>("\\s+");

        var winRanges = durations.Select((d, index) => CalculateWinRange(d, records[index]));

        var numberOfWaysToWin = winRanges.Select(r => r.End - r.Start + 1);

        return numberOfWaysToWin.Product().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var lines = input.Lines();
        var duration = lines[0].Replace("Time:", "").Replace(" ", "").ToNumber<long>();
        var record = lines[1].Replace("Distance:", "").Replace(" ", "").ToNumber<long>();

        var winrange = CalculateWinRange(duration, record);

        return (winrange.End - winrange.Start + 1).ToString();
    }

    /*
     * Solution:
     * 
     * Let T be the race duration, and R the current record.
     * 
     * The distance we travel by holding the button fot t milliseconds is:
     * 
     * D_T(t) = t*(T-t) = t*T - t^2
     * 
     * We have to solve D_T(t) > R for t.
     * 
     * We do this by calculating the roots of t^2 - t*T + R = 0
     */
    private (long Start, long End) CalculateWinRange(double raceDuration, double currentRecord)
    {
        var root1 = (raceDuration + Math.Sqrt(raceDuration * raceDuration - 4 * currentRecord)) / 2;
        var root2 = (raceDuration - Math.Sqrt(raceDuration * raceDuration - 4 * currentRecord)) / 2;

        var minRoot = Math.Min(root1, root2);
        var maxRoot = Math.Max(root1, root2);

        // Note the +- 1. This is because we do not win if we match the record exactly
        var intRoot1 = (long)Math.Floor(minRoot + 1);
        var intRoot2 = (long)Math.Ceiling(maxRoot - 1);

        return (intRoot1, intRoot2);
    }
}
