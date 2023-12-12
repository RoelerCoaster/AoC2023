using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day12;

internal class Day12 : DayBase
{
    public override int Day => 12;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var arangementCounts = input.Lines()
            .Select(line =>
            {
                var split = line.Split(" ");
                return AnalyzeArrangements(split[0], split[1].NumbersBySeparator<int>(",").ToList());
            })
            .ToList();

        return arangementCounts.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var arangementCounts = input.Lines()
            .Select(line =>
            {
                var split = line.Split(" ");
                var springs = string.Join('?', Enumerable.Repeat(split[0], 5));
                var numbers = string.Join(',', Enumerable.Repeat(split[1], 5));
                return AnalyzeArrangements(springs, numbers.NumbersBySeparator<int>(",").ToList());
            })
            .ToList();

        return arangementCounts.Sum().ToString();
    }

    private int AnalyzeArrangements(string row, List<int> groups)
    {
        var trimmed = row.Trim('.');

        if (groups.Count == 0)
        {
            if (!trimmed.Contains('#'))
            {
                // Valid arrangement found. Row is processed and no further groups need to be found.
                // Any '?' should be '.'
                return 1;
            }
            else
            {
                // Invalid solution. There are still damaged springs, but no groups to be processed
                return 0;
            }
        }

        if (trimmed.Length == 0 && groups.Count != 0)
        {
            // Invalid solution, there are still groups but no springs to be processed
            return 0;
        }

        if (groups.Sum() > trimmed.Count(c => c != '.'))
        {
            // Invalid solution, there are not damaged or unknown items to fill all groups
            return 0;
        }

        if (trimmed[0] == '?')
        {
            // first character is unknown. Simply try both possibilities.
            return AnalyzeArrangements('#' + trimmed.Substring(1), groups) +
                AnalyzeArrangements(trimmed.Substring(1), groups);

        }

        if (trimmed[0] != '#')
        {
            throw new InvalidOperationException($"Invalid character encountered; {trimmed[0]}");
        }

        // Check for a valid solution. We must match the first group, so the string must start
        // with the right number of damaged or unknown springs, followed by an operational or an unknown spring.
        var firstGroup = groups[0];
        var match = Regex.Match(trimmed, $"^[#?]{{{firstGroup}}}([.?]|$)");
        if (match.Success)
        {
            return AnalyzeArrangements(trimmed.Substring(match.Length), groups.Skip(1).ToList());
        }

        // Invalid solution. Group could not be matched.
        return 0;

    }
}
