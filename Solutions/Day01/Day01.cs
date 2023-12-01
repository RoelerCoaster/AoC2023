using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day01;

internal class Day01 : DayBase
{
    public override int Day => 1;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var calibrationValues = input
            .Lines()
            .Select(line => line.Where(char.IsAsciiDigit).ToList())
            .Select(digits => int.Parse(digits[0].ToString() + digits[^1]))
            .ToList();

        return calibrationValues.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var digitMap = new Dictionary<string, string>
        {
           { "one", "1" },
           { "two", "2" },
           { "three", "3" },
           { "four",  "4" },
           { "five", "5" },
           { "six", "6" },
           { "seven", "7" },
           { "eight", "8" },
           { "nine", "9" }
        };


        var calibrationValues = input
            .Lines()
            .Select(line =>
            {
                var all = digitMap.Keys.Concat(digitMap.Values).ToList();

                var firstMatches = all.ToDictionary(d => d, d => line.IndexOf(d));
                var lastMatches = all.ToDictionary(d => d, d => line.LastIndexOf(d));


                var firstDigit = firstMatches.Where(kp => kp.Value >= 0)
                    .MinBy(kp => kp.Value)
                    .Key;

                var lastDigit = lastMatches.Where(kp => kp.Value >= 0)
                    .MaxBy(kp => kp.Value)
                    .Key;

                return (firstDigit.Length == 1 ? firstDigit : digitMap[firstDigit]) +
                        (lastDigit.Length == 1 ? lastDigit : digitMap[lastDigit]);
            })
            .Select(int.Parse)
            .ToList();

        return calibrationValues.Sum().ToString();
    }
}
