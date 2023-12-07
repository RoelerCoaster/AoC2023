
namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day07;

internal class HandV2 : HandBase
{
    public HandV2(IReadOnlyList<int> cards, int bid) : base(cards, bid)
    {
    }

    protected override HandType DetermineType()
    {
        var jokers = Cards.Count(c => c == 1);

        var cardCounts = Cards
            .Where(c => c != 1)
            .GroupBy(c => c)
            .ToDictionary(grp => grp.Key, grp => grp.Count());

        if (jokers == 5 || cardCounts.ContainsValue(5 - jokers))
        {
            return HandType.FiveOfAKind;
        }

        if (cardCounts.ContainsValue(4 - jokers))
        {
            return HandType.FourOfAKind;
        }

        if (cardCounts.Any(kp1 => cardCounts.Any(kp2 => kp1.Key != kp2.Key && kp1.Value + kp2.Value + jokers == 5)))
        {
            return HandType.FullHouse;
        }

        if (cardCounts.ContainsValue(3 - jokers))
        {
            return HandType.ThreeOfAKind;
        }

        if (cardCounts.Any(kp1 => cardCounts.Any(kp2 => kp1.Key != kp2.Key && kp1.Value + kp2.Value + jokers == 4)))
        {
            return HandType.TwoPair;
        }

        if (cardCounts.ContainsValue(2 - jokers))
        {
            return HandType.OnePair;
        }

        return HandType.HighCard;
    }
}
