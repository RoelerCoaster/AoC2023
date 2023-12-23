using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day23;

internal class Day23 : DayBase
{
    public override int Day => 23;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();

        var startCol = Array.FindIndex(grid[0], x => x is '.');

        return BruteForceLongestPath(grid, new(0, startCol)).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = Regex.Replace(input, "[><^v]", ".").Grid();

        var startCol = Array.FindIndex(grid[0], x => x is '.');
        var start = new GridCoordinate(0, startCol);
        var reduced = ReduceGridToGraph(grid, start, GetNeighbors(grid, start).Single());

        return BruteForceReducedGraphLongestPath(reduced, start, new()).ToString();
    }

    private int BruteForceLongestPath(char[][] grid, GridCoordinate current)
    {
        if (current.Row == grid.Length - 1)
        {
            return 0;
        }

        var max = int.MinValue;

        var neighbors = GetNeighbors(grid, current).ToList();
        var currentSymbol = grid[current.Row][current.Col];
        grid[current.Row][current.Col] = 'O';
        foreach (var n in neighbors)
        {
            var length = BruteForceLongestPath(grid, n);
            max = Math.Max(length, max);
        }
        grid[current.Row][current.Col] = currentSymbol;

        return 1 + max;
    }

    private int BruteForceReducedGraphLongestPath(Dictionary<GridCoordinate, HashSet<(GridCoordinate, int)>> adjacency, GridCoordinate current, HashSet<GridCoordinate> visited)
    {
        var nonVisitedAdjacent = adjacency[current]
            .Where(x => !visited.Contains(x.Item1))
            .ToList();

        if (nonVisitedAdjacent.Count == 0)
        {
            if (adjacency[current].Count == 1)
            {
                return 0;
            }
            // invalid
            return int.MinValue;
        }

        var max = int.MinValue;
        visited.Add(current);
        foreach (var (n, l) in nonVisitedAdjacent)
        {
            var length = l + BruteForceReducedGraphLongestPath(adjacency, n, visited);
            max = Math.Max(length, max);
        }

        visited.Remove(current);

        return max;
    }

    private Dictionary<GridCoordinate, HashSet<(GridCoordinate, int)>> ReduceGridToGraph(char[][] grid, GridCoordinate parent, GridCoordinate next)
    {
        var result = new Dictionary<GridCoordinate, HashSet<(GridCoordinate, int)>>
        {
            { parent, new() }
        };
        ReduceGridToGraphSub(grid, parent, next, result);

        var copy = result.ToDictionary(x => x.Key, x => x.Value.ToHashSet());

        foreach (var kp in result)
        {
            foreach (var t in kp.Value)
            {
                if (!copy.ContainsKey(t.Item1))
                {
                    copy[t.Item1] = new();
                }

                copy[t.Item1].Add((kp.Key, t.Item2));
            }
        }

        return copy;
    }

    private void ReduceGridToGraphSub(char[][] grid, GridCoordinate parent, GridCoordinate next, Dictionary<GridCoordinate, HashSet<(GridCoordinate, int)>> result)
    {
        var pathLength = 1;

        List<GridCoordinate> neighbors;
        var current = next;
        var prev = parent;
        for (; ; )
        {
            if (current.Row == grid.Length - 1)
            {
                result[parent].Add((current, pathLength));
                return;
            }

            neighbors = GetNeighbors(grid, current)
                .Where(n => n != prev)
                .ToList();
            if (neighbors.Count > 1)
            {
                break;
            }
            pathLength++;
            prev = current;
            current = neighbors.Single();
        }


        result[parent].Add((current, pathLength));

        if (!result.ContainsKey(current))
        {
            result[current] = new();
            foreach (var n in neighbors)
            {
                ReduceGridToGraphSub(grid, current, n, result);
            }
        }
    }

    private IEnumerable<GridCoordinate> GetNeighbors(char[][] grid, GridCoordinate current)
    {
        switch (grid[current.Row][current.Col])
        {
            case '>':
                yield return current.CoordinateInDirection(CardinalDirection.East);
                break;
            case '<':
                yield return current.CoordinateInDirection(CardinalDirection.West);
                break;
            case '^':
                yield return current.CoordinateInDirection(CardinalDirection.North);
                break;
            case 'v':
                yield return current.CoordinateInDirection(CardinalDirection.South);
                break;
            case '.':
                {
                    var other = new[]
                    {
                        current.CoordinateInDirection(CardinalDirection.East),
                        current.CoordinateInDirection(CardinalDirection.West),
                        current.CoordinateInDirection(CardinalDirection.North),
                        current.CoordinateInDirection(CardinalDirection.South)
                    }
                    .Where(c => c.Row >= 0 && c.Col >= 0 && c.Row < grid.Length && c.Col < grid[0].Length);
                    foreach (var c in other)
                    {
                        if (grid[c.Row][c.Col] is not '#')
                        {
                            yield return c;

                        }
                    }

                    break;
                }
        }
    }
}
