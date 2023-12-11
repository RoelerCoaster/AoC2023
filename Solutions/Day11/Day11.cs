using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day11;

internal class Day11 : DayBase
{
    public override int Day => 11;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var (emptyRows, emptyColumns, galaxies) = ParseImageData(input.Grid());

        var distances = galaxies.UnorderedPairs(true)
            .Select(pair => Distance(pair.Item1, pair.Item2, emptyRows, emptyColumns, 2));

        return distances.Sum().ToString();

    }

    protected override async Task<string> SolvePart2(string input)
    {
        var (emptyRows, emptyColumns, galaxies) = ParseImageData(input.Grid());

        var distances = galaxies.UnorderedPairs(true)
            .Select(pair => Distance(pair.Item1, pair.Item2, emptyRows, emptyColumns, 1_000_000));

        return distances.Sum().ToString();
    }

    private ImageData ParseImageData(char[][] grid)
    {
        var emptyRows = Enumerable.Range(0, grid.Length).ToHashSet();
        var emptyColumns = Enumerable.Range(0, grid[0].Length).ToHashSet();
        var galaxies = new HashSet<GridCoordinate>();

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] == '#')
                {
                    emptyRows.Remove(row);
                    emptyColumns.Remove(col);
                    galaxies.Add(new GridCoordinate(row, col));
                }
            }
        }

        return new ImageData(emptyRows, emptyColumns, galaxies);
    }

    private long Distance(GridCoordinate a, GridCoordinate b, ISet<int> emptyRows, ISet<int> emptyColumns, int emptyExpansion)
    {
        var minRow = Math.Min(a.Row, b.Row);
        var maxRow = Math.Max(a.Row, b.Row);
        var minCol = Math.Min(a.Col, b.Col);
        var maxCol = Math.Max(a.Col, b.Col);
        long numberOfEmptyRows = emptyRows.Count(r => minRow < r && r < maxRow);
        long numberOfEmptyColumns = emptyColumns.Count(c => minCol < c && c < maxCol);

        return maxRow - minRow + maxCol - minCol + numberOfEmptyRows * (emptyExpansion - 1) + numberOfEmptyColumns * (emptyExpansion - 1);
    }

}


internal record ImageData(ISet<int> EmptyRows, ISet<int> EmptyColumns, ISet<GridCoordinate> Galaxies);
