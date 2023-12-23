using IntervalTree;
using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day22;

internal class Day22 : DayBase
{
    public override int Day => 22;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var bricks = input.Lines().Select(ParseBrick).ToList();

        var (supporting, supportedBy) = DetermineSupport(bricks);

        var nonRemovableCount = Enumerable.Range(0, bricks.Count)
            .Count(b => supporting[b].Any(s => supportedBy[s].Count == 1));

        return (bricks.Count - nonRemovableCount).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var bricks = input.Lines().Select(ParseBrick).ToList();

        var (supporting, supportedBy) = DetermineSupport(bricks);

        var falling = GetNumberOfFallingBricks(supporting, supportedBy);

        return falling.Sum().ToString();
    }

    private int[] GetNumberOfFallingBricks(HashSet<int>[] supporting, HashSet<int>[] supportedBy)
    {
        return Enumerable.Range(0, supporting.Length)
            .Select(b => GetNumberOfFallingBricks([b], supporting, supportedBy))
            .ToArray();
    }

    private int GetNumberOfFallingBricks(HashSet<int> bricksToRemove, HashSet<int>[] supporting, HashSet<int>[] supportedBy)
    {
        var fallingBricksDirectlyAbove = bricksToRemove.SelectMany(b => supporting[b])
            .Distinct()
            .Where(s => supportedBy[s].IsSubsetOf(bricksToRemove))
            .ToHashSet();

        if (fallingBricksDirectlyAbove.Count == 0)
        {
            return 0;
        }

        var newSupportedBy = supportedBy
            .Select(s =>
            {
                var newSet = new HashSet<int>(s);
                newSet.RemoveWhere(x => bricksToRemove.Contains(x));
                return newSet;
            })
            .ToArray();

        return fallingBricksDirectlyAbove.Count + GetNumberOfFallingBricks(fallingBricksDirectlyAbove, supporting, newSupportedBy);
    }

    private (HashSet<int>[] Supporting, HashSet<int>[] SupportedBy) DetermineSupport(List<Brick> bricks)
    {
        var lowToHigh = bricks
            .OrderBy(b => b.Start.Z)
            .ThenBy(b => b.End.Z);

        var xIntervalTree = new IntervalTree<int, Brick>();
        var yIntervalTree = new IntervalTree<int, Brick>();

        var supportedBy = new HashSet<int>[bricks.Count];
        var supporting = bricks.Select(_ => new HashSet<int>()).ToArray();


        foreach (var brick in lowToHigh)
        {
            var overlapX = xIntervalTree.Query(brick.Start.X, brick.End.X).ToHashSet();
            var overlapY = yIntervalTree.Query(brick.Start.Y, brick.End.Y);
            overlapX.IntersectWith(overlapY);
            var overlap = overlapX;

            Brick fallenBrick;
            if (overlap.Count == 0)
            {
                fallenBrick = brick with
                {
                    Start = brick.Start with { Z = 1 },
                    End = brick.End with { Z = brick.End.Z - brick.Start.Z + 1 }
                };

                supportedBy[brick.Index] = new();
            }
            else
            {

                var highest = overlap.GroupBy(b => b.End.Z)
                    .MaxBy(grp => grp.Key)!
                    .ToList();

                var highestZ = highest.First().End.Z;

                fallenBrick = brick with
                {
                    Start = brick.Start with { Z = highestZ + 1 },
                    End = brick.End with { Z = brick.End.Z - brick.Start.Z + highestZ + 1 }
                };

                supportedBy[brick.Index] = highest.Select(b => b.Index).ToHashSet();
                foreach (var b in highest)
                {
                    supporting[b.Index].Add(brick.Index);
                }
            }

            xIntervalTree.Add(fallenBrick.Start.X, fallenBrick.End.X, fallenBrick);
            yIntervalTree.Add(fallenBrick.Start.Y, fallenBrick.End.Y, fallenBrick);
        }

        return (supporting, supportedBy);
    }

    private Brick ParseBrick(string line, int index)
    {
        var parts = line.Split("~");
        var start = parts[0].NumbersBySeparator<int>(",");
        var end = parts[1].NumbersBySeparator<int>(",");

        return new(new(start[0], start[1], start[2]), new(end[0], end[1], end[2]), index);
    }
}

internal record Coordinate3D(int X, int Y, int Z);

internal record Brick(Coordinate3D Start, Coordinate3D End, int Index);