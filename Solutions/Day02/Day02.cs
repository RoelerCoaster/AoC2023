using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day02;

using Grab = (int Red, int Green, int Blue);

internal record Game(int Id, List<Grab> grabs);

internal class Day02 : DayBase
{

    public override int Day => 2;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var games = input.Lines().Select(ParseGame);

        var possibleGames = games
            .Where(
                game => game.grabs.All(grab => grab is ( <= 12, <= 13, <= 14))
            );

        return possibleGames.Sum(p => p.Id).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var games = input.Lines().Select(ParseGame);

        var powers = games
            .Select(game =>
            {
                var minRed = game.grabs.Max(g => g.Red);
                var minGreen = game.grabs.Max(g => g.Green);
                var minBlue = game.grabs.Max(g => g.Blue);

                return minRed * minGreen * minBlue;
            });

        return powers.Sum().ToString();

    }

    private Game ParseGame(string line)
    {
        var parts = line.Split(":");

        var gamePart = parts[0];
        var grabsPart = parts[1];

        var id = int.Parse(Regex.Match(gamePart, "\\d+").Value);

        var grabs = grabsPart
            .Split(";")
            .Select(ParseGrab)
            .ToList();

        return new Game(id, grabs);
    }

    private Grab ParseGrab(string grabString)
    {
        var parts = grabString.Split(",", StringSplitOptions.TrimEntries);

        return (
            ParseColorCount("red", parts),
            ParseColorCount("green", parts),
            ParseColorCount("blue", parts)
        );

    }

    private int ParseColorCount(string color, string[] grabParts)
    {
        var colorPart = grabParts.FirstOrDefault(p => p.Contains(color));
        return colorPart == null
            ? 0
            : int.Parse(Regex.Match(colorPart, "\\d+").Value);
    }
}
