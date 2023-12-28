using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day21;

internal class Day21 : DayBase
{
    public override int Day => 21;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var reachable = WalkGridV2(input.Grid(), 64, false);

        return reachable.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        // Very conveniently, the real grid is 131 x 131.
        // which is 26_501_365 = 202_300 * 131 + 65 (half a grid from the start)
        // So in the infinite grid, we have to repeat the grid 202_300 times in each direction.
        //
        // Also very conveniently, the edges can be reached from the start without obstruction.
        var grid = input.Grid();


        var r65 = WalkGridV2(grid, 65, true);
        var r262 = WalkGridV2(grid, 262, true);
        var r131 = WalkGridV2(grid, 131, true);
        var r64 = WalkGridV2(grid, 64, true);

        var n1 = WalkGridV2(grid, 65 + 1 * 262, true);

        var leftover = n1 - r262 - 3 * r65;

        long Formula(long n)
        {
            // the last term was found experimentally. Honestly, I have no idea.....
            return n * n * r262 + (2 * n + 1) * r65 + n * leftover - (n * (n - 1) / 2 * 898);
        }

        //var test1 = Formula(1);
        //var d1 = n1 - test1;

        //var n2 = WalkGridV2(grid, 65 + 2 * 262, true);
        //var test2 = Formula(2);
        //var d2 = n2 - test2;

        //var n3 = WalkGridV2(grid, 65 + 3 * 262, true);
        //var test3 = Formula(3);
        //var d3 = n3 - test3;


        //var n4 = WalkGridV2(grid, 65 + 4 * 262, true);
        //var test4 = Formula(4);
        //var d4 = n4 - test4;


        //var n5 = WalkGridV2(grid, 65 + 5 * 262, true);
        //var test5 = Formula(5);
        //var d5 = n5 - test5;


        return Formula(202_300 / 2).ToString();
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
                foreach (var adjacent in GetAdjacent(coordinate, grid, false))
                {
                    nextToVisit.Add(adjacent);
                }
            }
            toVisit = nextToVisit;
        }

        return toVisit;
    }

    private int WalkGridV2(char[][] grid, int steps, bool infinite)
    {
        var distances = new Dictionary<GridCoordinate, int>();
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
        while (toVisit.Count != 0 && step <= steps)
        {
            var nextToVisit = new HashSet<GridCoordinate>();

            foreach (var coordinate in toVisit)
            {
                distances[coordinate] = step;

                foreach (var adjacent in GetAdjacent(coordinate, grid, infinite))
                {
                    if (!distances.ContainsKey(adjacent))
                    {
                        nextToVisit.Add(adjacent);
                    }
                }
            }
            toVisit = nextToVisit;
            step++;
        }

        // If we can reach a plot within the required number of steps, then we can simply walk back and forth between
        // two neigbouring cells. So the remaining number of steps must be even.

        return distances
            .Values
            .Count(d =>
            {
                var diff = steps - d;
                return diff >= 0 && diff % 2 == 0;
            });
    }


    private IEnumerable<GridCoordinate> GetAdjacent(GridCoordinate coordinate, char[][] grid, bool infinite)
    {
        return new[]
        {
            coordinate.CoordinateInDirection(CardinalDirection.North),
            coordinate.CoordinateInDirection(CardinalDirection.East),
            coordinate.CoordinateInDirection(CardinalDirection.South),
            coordinate.CoordinateInDirection(CardinalDirection.West),
        }.Where(c =>
            (infinite || (c.Row >= 0 && c.Col >= 0 && c.Row < grid.Length && c.Col < grid[0].Length))
            && grid[MathUtil.Mod(c.Row, grid.Length)][MathUtil.Mod(c.Col, grid[0].Length)] is not '#'
        );
    }
}
