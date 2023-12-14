using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day14;

internal class Day14 : DayBase
{
    public override int Day => 14;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    private static string[] CycleOrder = ["north", "west", "south", "east"];

    protected override async Task<string> SolvePart1(string input)
    {
        return GetTotalNorthLoadAfterNorthTilt(input.Grid()).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();

        var gridToCycle = new Dictionary<string, int> { { input, 0 } };
        var cycleToGrid = new Dictionary<int, string> { { 0, input } };

        var cycleStart = 0;
        for (var i = 1; i <= 1_000_000_000; i++)
        {
            Cycle(grid);
            var gridString = ToString(grid);

            if (gridToCycle.TryGetValue(gridString, out cycleStart))
            {
                break;
            }
            gridToCycle.Add(gridString, i);
            cycleToGrid.Add(i, gridString);
        }

        var maxCycle = cycleToGrid.Keys.Max();
        var cycleLength = maxCycle - cycleStart + 1;
        var resultIndex = (1_000_000_000 - cycleStart) % cycleLength + cycleStart;

        var resultGrid = cycleToGrid[resultIndex];


        return GetLoad(resultGrid.Grid()).ToString();
    }

    private int GetTotalNorthLoadAfterNorthTilt(char[][] grid)
    {
        return Enumerable.Range(0, grid[0].Length)
            .Select(c => GetColumnLoad(grid, c))
            .Sum();
    }

    private int GetColumnLoad(char[][] grid, int col)
    {
        var totalLoad = 0;
        var loadAtCurrentPosition = grid.Length;
        for (var r = 0; r < grid.Length; r++)
        {
            switch (grid[r][col])
            {
                case 'O':
                    totalLoad += loadAtCurrentPosition;
                    loadAtCurrentPosition--;
                    break;
                case '.':
                    // No-op. Any rolling rocks south of this position will roll to this position.
                    break;
                case '#':
                    // Any rolling rocks will get "stuck" south of this position
                    loadAtCurrentPosition = grid.Length - r - 1;
                    break;
                default:
                    throw new InvalidOperationException("Unknown symbol");
            }
        }

        return totalLoad;
    }

    private void Cycle(char[][] grid)
    {
        foreach (var d in CycleOrder)
        {
            Tilt(grid, d);
        }
    }

    private void Tilt(char[][] grid, string direction)
    {

        Func<int, int, char> gridGetter = direction switch
        {
            "north" => (outer, inner) => grid[inner][outer],
            "west" => (outer, inner) => grid[outer][inner],
            "south" => (outer, inner) => grid[^(inner + 1)][outer],
            "east" => (outer, inner) => grid[outer][^(inner + 1)],
            _ => throw new InvalidOperationException()
        };

        Action<int, int, char> gridSetter = direction switch
        {
            "north" => (outer, inner, value) => grid[inner][outer] = value,
            "west" => (outer, inner, value) => grid[outer][inner] = value,
            "south" => (outer, inner, value) => grid[^(inner + 1)][outer] = value,
            "east" => (outer, inner, value) => grid[outer][^(inner + 1)] = value,
            _ => throw new InvalidOperationException()
        };

        var outerLimit = direction is "north" or "south" ? grid[0].Length : grid.Length;
        var innerLimit = direction is "north" or "south" ? grid.Length : grid[0].Length;

        void RollStones(int outer, int offset, int rollingCounter, int max)
        {
            for (var i = offset; i < rollingCounter + offset; i++)
            {
                gridSetter(outer, i, 'O');
            }
            for (var i = offset + rollingCounter; i < max; i++)
            {
                gridSetter(outer, i, '.');
            }
        }

        for (var outer = 0; outer < outerLimit; outer++)
        {
            var offset = 0;
            var rollingCounter = 0;

            for (var inner = 0; inner < innerLimit; inner++)
            {
                switch (gridGetter(outer, inner))
                {
                    case 'O':
                        rollingCounter++;
                        break;
                    case '.':
                        break;
                    case '#':
                        RollStones(outer, offset, rollingCounter, inner);
                        offset = inner + 1;
                        rollingCounter = 0;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown symbol");
                }
            }

            RollStones(outer, offset, rollingCounter, innerLimit);
        }
    }

    private int GetLoad(char[][] grid)
    {
        var totalLoad = 0;
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                if (grid[r][c] is 'O')
                {
                    totalLoad += (grid.Length - r);
                }
            }
        }

        return totalLoad;
    }

    private string ToString(char[][] grid)
    {
        return grid.Select(c => c.StringJoin("")).StringJoin(Environment.NewLine);
    }
}
