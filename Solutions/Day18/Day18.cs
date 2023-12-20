using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day18;

internal class Day18 : DayBase
{
    public override int Day => 18;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var actions = input.Lines()
           .Select(l =>
           {
               var split = l.Split(" ");
               return (CharToDirection(split[0][0]), split[1].ToNumber<int>());
           })
           .ToArray();

        var edgeCorners = TraceEdgeCorners(actions);
        var area = GetAreaV2(edgeCorners);

        return area.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var directionMap = new[] { 'R', 'D', 'L', 'U' };
        var actions = input.Lines()
           .Select(l =>
           {
               var split = l.Split(" ");

               var cleaned = split[2].Trim('#', '(', ')');

               var steps = Convert.ToInt32(cleaned.Substring(0, 5), 16);
               var direction = CharToDirection(directionMap[cleaned.Substring(5).ToNumber<int>()]);

               return (direction, steps);
           })
           .ToArray();

        var edgeCorners = TraceEdgeCorners(actions);
        var area = GetAreaV2(edgeCorners);

        return area.ToString();
    }

    private List<GridCoordinate> TraceEdgeCorners((CardinalDirection Direction, int steps)[] actions)
    {
        var current = new GridCoordinate(0, 0);
        var edge = new List<GridCoordinate> { current };

        foreach (var (direction, steps) in actions)
        {
            current = current.CoordinateInDirection(direction, steps);
            edge.Add(current);
        }

        return edge;
    }

    private long GetAreaV2(List<GridCoordinate> edgeCorners)
    {
        var cornerGroups = edgeCorners
            .Distinct()
            .GroupBy(e => e.Row)
            .OrderBy(g => g.Key)
            .Select(g => (Row: g.Key, Corners: g.OrderBy(c => c.Col).ToList()))
            .ToList();

        var firstGroup = cornerGroups[0];

        var initialRanges = firstGroup.Corners.Batch(2).Select(batch => new Range(batch[0].Col, batch[1].Col));

        var activeRanges = new List<Range>(initialRanges);
        long area = initialRanges.Sum(r => r.Length);
        var prevRow = firstGroup.Row;

        foreach (var (row, corners) in cornerGroups[1..])
        {
            long currentActiveRangesLength = activeRanges.Sum(r => r.Length);

            area += currentActiveRangesLength * (row - prevRow - 1);

            var columnIndices = activeRanges
                .SelectMany(r => new[] { r.Start, r.End })
                .Concat(
                    corners.Select(c => c.Col)
                ).Order();

            var unmerged = columnIndices
                .Batch(2)
                .Select(batch => new Range(batch[0], batch[1]))
                .Where(r => r.Length > 1)
                .ToList();

            var merged = MergeRanges(unmerged).ToList();

            area += MergeRanges(merged.Concat(activeRanges)).Sum(r => r.Length);

            activeRanges = merged;
            prevRow = row;
        }

        return area;
    }

    private IEnumerable<Range> MergeRanges(IEnumerable<Range> ranges)
    {
        var sortedData = ranges.SelectMany<Range, (int index, bool isEnd)>(r => new[] { (r.Start, false), (r.End, true) })
            .OrderBy(x => x.index)
            .ThenBy(x => x.isEnd)
            .ToList();

        int? start = null;
        var startCount = 0;

        foreach (var (index, isEnd) in sortedData)
        {
            if (start == null)
            {
                start = index;
            }

            startCount += isEnd ? -1 : 1;

            if (startCount == 0)
            {
                yield return new Range(start.Value, index);
                start = null;
            }
        }
    }

    private CardinalDirection CharToDirection(char c)
    {
        return c switch
        {
            'R' => CardinalDirection.East,
            'L' => CardinalDirection.West,
            'U' => CardinalDirection.North,
            'D' => CardinalDirection.South,
            _ => throw new NotSupportedException()
        };
    }
}

internal record Range(int Start, int End)
{
    public int Length => End - Start + 1;
}
