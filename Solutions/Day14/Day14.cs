using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day14;

internal class Day14 : DayBase
{
    public override int Day => 14;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        return GetTotalNorthLoad(input.Grid()).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    protected int GetTotalNorthLoad(char[][] grid)
    {
        return Enumerable.Range(0, grid[0].Length)
            .Select(c => GetColumnLoad(grid, c))
            .Sum();
    }

    protected int GetColumnLoad(char[][] grid, int col)
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
}
