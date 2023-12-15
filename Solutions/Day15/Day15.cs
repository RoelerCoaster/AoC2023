using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day15;

internal class Day15 : DayBase
{
    public override int Day => 15;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sequence = input.Split(',');

        var hashes = sequence.Select(Hash).ToList();

        return hashes.Sum().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var hashTable = new LinkedList<LabeledLens>[256];

        var sequence = input.Split(',');

        foreach (var instruction in sequence)
        {
            Execute(hashTable, instruction);
        }

        var power = 0;

        for (var i = 0; i < hashTable.Length; i++)
        {
            var box = hashTable[i];
            if (box is null)
            {
                continue;
            }

            var j = 1;
            foreach (var lens in box)
            {
                power += (i + 1) * j * lens.FocalLength;
                j++;
            }
        }


        return power.ToString();
    }

    private int Hash(string input)
    {
        var currentValue = 0;

        foreach (var c in input)
        {
            currentValue += c;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }

    private void Execute(LinkedList<LabeledLens>[] hashTable, string instruction)
    {
        if (instruction.Contains('='))
        {
            var split = instruction.Split('=');
            Insert(hashTable, split[0], split[1].ToNumber<int>());
        }
        else
        {
            Remove(hashTable, instruction.TrimEnd('-'));
        }
    }

    private void Insert(LinkedList<LabeledLens>[] hashTable, string label, int focalLength)
    {
        var hash = Hash(label);

        var lens = new LabeledLens(label, focalLength);

        if (hashTable[hash] is null)
        {
            hashTable[hash] = new();
        }

        var list = hashTable[hash];

        var node = list.Find(lens);

        if (node is null)
        {
            list.AddLast(lens);

        }
        else
        {
            list.AddAfter(node, lens);
            list.Remove(node);
        }

    }

    private void Remove(LinkedList<LabeledLens>[] hashTable, string label)
    {
        var hash = Hash(label);

        if (hashTable[hash] is null)
        {
            return;
        }

        var list = hashTable[hash];

        list.Remove(new LabeledLens(label));
    }
}

internal class LabeledLens : IEquatable<LabeledLens?>
{
    public string Label { get; }
    public int FocalLength { get; }

    public LabeledLens(string label) : this(label, 0)
    {
    }

    public LabeledLens(string label, int focalLength)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
        FocalLength = focalLength;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LabeledLens);
    }

    public bool Equals(LabeledLens? other)
    {
        return other is not null &&
               Label == other.Label;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Label);
    }

    public static bool operator ==(LabeledLens? left, LabeledLens? right)
    {
        return EqualityComparer<LabeledLens>.Default.Equals(left, right);
    }

    public static bool operator !=(LabeledLens? left, LabeledLens? right)
    {
        return !(left == right);
    }
};
