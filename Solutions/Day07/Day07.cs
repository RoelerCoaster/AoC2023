using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day07;

internal class Day07 : DayBase
{
    public override int Day => 7;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var hands = input.Lines()
            .Select(line =>
            {
                var split = line.Split(" ", StringSplitOptions.TrimEntries);
                var cards = split[0].Select(ParseCard).ToList();
                return new HandV1(cards, split[1].ToNumber<int>());
            })
            .ToList();

        var ranked = hands
            .OrderBy(h => h.Type);

        foreach (var index in Enumerable.Range(0, 5))
        {
            ranked = ranked.ThenBy(h => h.Cards[index]);
        }

        return ranked.Select((c, i) => (long)c.Bid * (i + 1)).Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var hands = input.Lines()
            .Select(line =>
            {
                var split = line.Split(" ", StringSplitOptions.TrimEntries);
                var cards = split[0].Select(ParseCardV2).ToList();
                return new HandV2(cards, split[1].ToNumber<int>());
            })
            .ToList();

        var ranked = hands
            .OrderBy(h => h.Type);

        foreach (var index in Enumerable.Range(0, 5))
        {
            ranked = ranked.ThenBy(h => h.Cards[index]);
        }

        return ranked.Select((c, i) => (long)c.Bid * (i + 1)).Sum().ToString();
    }

    private int ParseCard(char card)
    {
        return card switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ => card.ToString().ToNumber<int>()
        };
    }

    private int ParseCardV2(char card)
    {
        var parsed = ParseCard(card);
        return parsed == 11 ? 1 : parsed;
    }
}
