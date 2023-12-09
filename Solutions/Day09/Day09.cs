using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day09;

internal class Day09 : DayBase
{
    public override int Day => 9;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var predictions = input
            .Lines()
            .Select(l => l.NumbersBySeparator<int>(" "))
            .Select(PredictNext)
            .ToList();

        return predictions.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var predictions = input
                    .Lines()
                    .Select(l => l.NumbersBySeparator<int>(" "))
                    .Select(PredictPrevious)
                    .ToList();

        return predictions.Sum().ToString();
    }

    private int PredictNext(int[] history)
    {
        if (history.All(x => x == 0))
        {
            return 0;
        }

        var differences = history.Pairwise((a, b) => b - a).ToArray();

        var prediction = PredictNext(differences);

        return prediction + history[^1];
    }

    private int PredictPrevious(int[] history)
    {
        if (history.All(x => x == 0))
        {
            return 0;
        }

        var differences = history.Pairwise((a, b) => b - a).ToArray();

        var prediction = PredictPrevious(differences);

        return history[0] - prediction;
    }
}
