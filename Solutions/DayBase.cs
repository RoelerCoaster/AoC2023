using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using System.Diagnostics;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions;

internal abstract class DayBase
{
    public abstract int Day { get; }

    public abstract bool UseTestInput { get; }

    protected abstract PartToRun PartsToRun { get; }

    public bool IsActive => PartsToRun != PartToRun.None;

    public async Task<PartSolution> RunPart(PartToRun part, string input)
    {
        Solve solve = part switch
        {
            PartToRun.Part1 => SolvePart1,
            PartToRun.Part2 => SolvePart2,
            _ => throw new InvalidOperationException("Invalid part value")
        };

        return await RunPart(part, solve, input);
    }

    private async Task<PartSolution> RunPart(PartToRun part, Solve solve, string input)
    {
        if (!PartsToRun.HasFlag(part))
        {
            return PartSolution.Skipped();
        }

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await solve(input);
            stopwatch.Stop();

            return PartSolution.Valid(result, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            return PartSolution.Error(ex);
        }
    }

    protected abstract Task<string> SolvePart1(string input);

    protected abstract Task<string> SolvePart2(string input);

    private delegate Task<string> Solve(string input);
}
