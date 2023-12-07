namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day07;

internal abstract class HandBase
{
    public int Bid { get; }
    public IReadOnlyList<int> Cards { get; }

    public HandType Type { get; }

    public HandBase(IReadOnlyList<int> cards, int bid)
    {
        Cards = cards;
        Bid = bid;
        Type = DetermineType();
    }

    protected abstract HandType DetermineType();
}