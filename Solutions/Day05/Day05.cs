using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day05;

internal class Day05 : DayBase
{
    public override int Day => 5;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections().ToList();

        var seeds = ParseSeeds(sections[0]);

        var categoryMaps = sections.Skip(1).Select(ParseCategoryMap).ToList();

        var seedLocations = seeds.Select(seed =>
        {
            var current = seed;
            foreach (var map in categoryMaps)
            {
                current = map.ToDest(current);
            }

            return current;
        });

        return seedLocations.Min().ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections().ToList();

        var seedRanges = ParseSeedRanges(sections[0]);

        var categoryMaps = sections.Skip(1).Select(ParseCategoryMapWithDummyFillers).ToList();

        var locationRanges = seedRanges.SelectMany(seedRange =>
        {
            List<SeedRange> current = new() { seedRange };
            foreach (var map in categoryMaps)
            {
                current = map.Items.SelectMany(item => current.Select(r => Apply(r, item)))
                    .NotNull()
                    .ToList();
            }

            return current;
        });

        return locationRanges.Min(r => r.Start).ToString();
    }

    private long[] ParseSeeds(string seedsLine)
    {
        return seedsLine.Replace("seeds: ", "").NumbersBySeparator<long>(" ");
    }

    private List<SeedRange> ParseSeedRanges(string seedsLine)
    {
        return ParseSeeds(seedsLine)
            .Batch(2)
            .Select(b => new SeedRange(b[0], b[1]))
            .ToList();
    }

    private CategoryMap ParseCategoryMap(string section)
    {
        var lines = section.Lines().ToList();

        var headerMatch = Regex.Match(lines[0], "(?<src>\\w+)-to-(?<dest>\\w+)");
        var src = headerMatch.Groups["src"].Value;
        var dest = headerMatch.Groups["dest"].Value;
        var items = lines.Skip(1).Select(ParseItem).ToList();

        return new CategoryMap(src, dest, items);
    }

    private CategoryMapItem ParseItem(string itemLine)
    {
        var split = itemLine.NumbersBySeparator<long>(" ");
        return new CategoryMapItem(split[0], split[1], split[2]);
    }

    private CategoryMap ParseCategoryMapWithDummyFillers(string section)
    {
        var map = ParseCategoryMap(section);

        var orderedItems = map.Items
            .OrderBy(m => m.Source)
            .ToList();

        var head = new CategoryMapItem(0, 0, orderedItems[0].Source);
        var lastSourcEnd = orderedItems[^1].SourceEnd;
        var tail = new CategoryMapItem(lastSourcEnd, lastSourcEnd, long.MaxValue - lastSourcEnd - 1);

        var itemsToAdd = orderedItems
            .Pairwise((first, second) => new CategoryMapItem(first.SourceEnd + 1, first.SourceEnd + 1, second.Source - first.SourceEnd - 1))
            .Where(i => i.Length > 0)
            .ToList();

        List<CategoryMapItem> newItems = [
            head,
            .. orderedItems,
            .. itemsToAdd,
            tail
        ];

        return map with { Items = newItems.OrderBy(m => m.Source).ToList() };
    }

    private SeedRange? Apply(SeedRange seedRange, CategoryMapItem item)
    {
        var start = Math.Max(seedRange.Start, item.Source);
        var end = Math.Min(seedRange.End, item.SourceEnd);

        var length = end - start + 1;

        if (length <= 0)
        {
            return null;
        }

        return new SeedRange(item.ToDest(start), length);
    }
}

internal record CategoryMapItem(long Destination, long Source, long Length)
{
    public bool HasSource(long value) => value >= Source && value < Source + Length;

    public long ToDest(long value) => value + Destination - Source;

    public long SourceEnd => Source + Length - 1;

    public long DestinationEnd => Destination + Length - 1;

}

internal record CategoryMap(string SourceCategory, string DestinationCategory, IReadOnlyList<CategoryMapItem> Items)
{
    public long ToDest(long value) => Items.FirstOrDefault(i => i.HasSource(value))?.ToDest(value) ?? value;

}

internal record SeedRange(long Start, long Length)
{
    public long End => Start + Length - 1;
}