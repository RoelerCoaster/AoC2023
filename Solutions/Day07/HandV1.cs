
namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day07;

internal class HandV1 : HandBase
{
    public HandV1(IReadOnlyList<int> cards, int bid) : base(cards, bid)
    {
    }

    protected override HandType DetermineType()
    {
        var cardCounts = Cards.GroupBy(x => x).Select(grp => grp.Count()).ToList();

        if (cardCounts.Contains(5))
        {
            return HandType.FiveOfAKind;
        }

        if (cardCounts.Contains(4))
        {
            return HandType.FourOfAKind;
        }

        if (cardCounts.Contains(3) && cardCounts.Contains(2))
        {
            return HandType.FullHouse;
        }

        if (cardCounts.Contains(3))
        {
            return HandType.ThreeOfAKind;
        }

        if (cardCounts.Count(x => x == 2) == 2)
        {
            return HandType.TwoPair;
        }

        if (cardCounts.Contains(2))
        {
            return HandType.OnePair;
        }

        return HandType.HighCard;
    }
}
