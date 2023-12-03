using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day03;

internal record NumberLabel(int Row, int Col, int Value);

internal class Day03 : DayBase
{
    public override int Day => 3;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var lines = input.Lines();

        var numbers = lines.SelectMany((line, index) => GetNumbersFromLine(index, line)).ToList();

        var grid = lines.Select(l => l.ToCharArray()).ToArray();

        var numbersAdjacentToSymbol = numbers
            .Where(n => GetCharsAroundNumber(grid, n).Any(c => !char.IsAsciiDigit(c.Value) && c.Value != '.'))
            .ToList();

        return numbersAdjacentToSymbol.Sum(n => n.Value).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var lines = input.Lines();

        var numbers = lines.SelectMany((line, index) => GetNumbersFromLine(index, line)).ToList();

        var grid = lines.Select(l => l.ToCharArray()).ToArray();

        var ratios = numbers
            .SelectMany(n => GetCharsAroundNumber(grid, n)
                                .Where(c => c.Value == '*')
                                .Select(c => new
                                {
                                    Number = n,
                                    SymbolWithLocation = c
                                })
            )
            .GroupBy(x => x.SymbolWithLocation, x => x.Number)
            // Each group now contains all numbers adjacent to the same symbol
            .Where(grp => grp.Count() == 2)
            .Select(grp => grp.Product(n => n.Value))
            .ToList();

        return ratios.Sum().ToString();
    }

    private IEnumerable<NumberLabel> GetNumbersFromLine(int row, string line)
    {
        var matches = Regex.Matches(line, "\\d+");

        return matches.Select(m => new NumberLabel(
            row,
            m.Index,
            int.Parse(m.Value)
        ));
    }

    private IEnumerable<(int Row, int Col, char Value)> GetCharsAroundNumber(char[][] grid, NumberLabel number)
    {
        var numberLength = number.Value.ToString().Length;
        var minCol = Math.Max(0, number.Col - 1);
        var maxCol = Math.Min(grid[0].Length - 1, number.Col + numberLength);


        if (number.Row > 0)
        {
            for (var c = minCol; c <= maxCol; c++)
            {
                yield return (number.Row - 1, c, grid[number.Row - 1][c]);
            }
        }

        if (minCol == number.Col - 1)
        {
            yield return (number.Row, minCol, grid[number.Row][minCol]);
        }

        if (maxCol == number.Col + numberLength)
        {
            yield return (number.Row, maxCol, grid[number.Row][maxCol]);
        }

        if (number.Row < grid.Length - 1)
        {
            for (var c = minCol; c <= maxCol; c++)
            {
                yield return (number.Row + 1, c, grid[number.Row + 1][c]);
            }
        }
    }
}
