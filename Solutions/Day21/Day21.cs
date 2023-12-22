using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day21;

internal class Day21 : DayBase
{
    public override int Day => 21;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var reachable = WalkGridV2(input.Grid(), 64);

        return reachable.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    private HashSet<GridCoordinate> WalkGridBruteForce(char[][] grid, int steps)
    {
        GridCoordinate? start = null;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 'S')
                {
                    start = new(r, c);
                }
            }
        }

        if (start is null)
        {
            throw new InvalidOperationException();
        }

        var toVisit = new HashSet<GridCoordinate> { start };

        for (var step = 1; step <= steps; step++)
        {
            var nextToVisit = new HashSet<GridCoordinate>();

            foreach (var coordinate in toVisit)
            {
                foreach (var adjacent in GetAdjacent(coordinate, grid))
                {
                    nextToVisit.Add(adjacent);
                }
            }
            toVisit = nextToVisit;
        }

        return toVisit;
    }

    private int WalkGridV2(char[][] grid, int steps)
    {
        var distances = grid.Select(row => row.Select(_ => int.MaxValue).ToArray()).ToArray();
        GridCoordinate? start = null;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 'S')
                {
                    start = new(r, c);
                }
            }
        }

        if (start is null)
        {
            throw new InvalidOperationException();
        }

        var toVisit = new HashSet<GridCoordinate> { start };

        var step = 0;
        while (toVisit.Count != 0)
        {
            var nextToVisit = new HashSet<GridCoordinate>();

            foreach (var coordinate in toVisit)
            {
                distances[coordinate.Row][coordinate.Col] = step;

                foreach (var adjacent in GetAdjacent(coordinate, grid))
                {
                    if (distances[adjacent.Row][adjacent.Col] == int.MaxValue)
                    {
                        nextToVisit.Add(adjacent);
                    }
                }
            }
            toVisit = nextToVisit;
            step++;
        }

        // If we can reach a plot within the required number of steps, then we can simply walk back and forth between
        // two neigbouring cells. so the remainder must be even.

        return distances.SelectMany(row => row).Count(d =>
        {
            var diff = steps - d;
            return diff >= 0 && diff % 2 == 0;
        });
    }


    private IEnumerable<GridCoordinate> GetAdjacent(GridCoordinate coordinate, char[][] grid)
    {
        return new[]
        {
            coordinate.CoordinateInDirection(CardinalDirection.North),
            coordinate.CoordinateInDirection(CardinalDirection.East),
            coordinate.CoordinateInDirection(CardinalDirection.South),
            coordinate.CoordinateInDirection(CardinalDirection.West),
        }.Where(c => c.Row >= 0 && c.Col >= 0 && c.Row < grid.Length && c.Col < grid[0].Length && grid[c.Row][c.Col] is not '#');
    }
}
