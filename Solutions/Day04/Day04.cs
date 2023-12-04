using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day04;

internal record ScratchCard(int Id, ISet<int> Winning, ISet<int> Owned);

internal class Day04 : DayBase
{
    public override int Day => 4;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var points = input
            .Lines()
            .Select(ParseCard)
            .Select(GetPoints)
            .ToList();

        return points.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        // Matches of id is at index id - 1
        var matches = input
            .Lines()
            .Select(ParseCard)
            .Select(card => card.Winning.Intersect(card.Owned).Count())
            .ToArray();

        // Number of cards of if is at index id - 1
        // Initially one card of each id each
        var numberOfCards = new int[matches.Length];
        Array.Fill(numberOfCards, 1);

        for (var i = 0; i < matches.Length; i++)
        {
            Enumerable.Range(i + 1, matches[i]).ForEach(nextCard =>
            {
                if (nextCard < numberOfCards.Length)
                {
                    numberOfCards[nextCard] += numberOfCards[i];
                }
            });
        }

        return numberOfCards.Sum().ToString();
    }

    private ScratchCard ParseCard(string line)
    {
        var parts = line.Split(new char[] { ':', '|' }, StringSplitOptions.TrimEntries);

        var id = Regex.Match(parts[0], "\\d+").Value.ToNumber<int>();
        var winning = parts[1].NumbersByRegex<int>("\\s+").ToHashSet();
        var owned = parts[2].NumbersByRegex<int>("\\s+").ToHashSet();

        return new ScratchCard(id, winning, owned);
    }

    private int GetPoints(ScratchCard card)
    {
        var numberOfmatches = card.Winning.Intersect(card.Owned).Count();

        return numberOfmatches == 0
            ? 0
            : 1 << (numberOfmatches - 1);
    }
}
