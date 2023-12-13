using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using System.Text;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day13;

internal class Day13 : DayBase
{
    public override int Day => 13;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var reflectionLines = input.Sections()
            .Select(section => new
            {
                Vertical = GetVerticalReflectionLine(section),
                Horizontal = GetHorizontalReflectionLine(section),
                Section = section
            })
            .ToList();

        return reflectionLines.Sum(l => l.Vertical + 100 * l.Horizontal).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var reflectionLines = input.Sections()
            .Select(section => new
            {
                Vertical = GetCorrectedVerticalReflectionLine(section),
                Horizontal = GetCorrectedHorizontalReflectionLine(section),
                Section = section
            })
            .ToList();

        return reflectionLines.Sum(l => l.Vertical + 100 * l.Horizontal).ToString();
    }

    private int GetVerticalReflectionLine(string section)
    {
        var indicesPerLine = section.Lines().Select(GetReflectionIndicesForLine).ToList();

        var intersection = new HashSet<int>(indicesPerLine.First());
        foreach (var item in indicesPerLine)
        {
            intersection.IntersectWith(item);
        }

        return intersection.SingleOrDefault(0);
    }

    public ISet<int> GetReflectionIndicesForLine(string line)
    {
        var indices = new HashSet<int>();

        for (var i = 1; i < line.Length; i++)
        {
            if (LineCanBeReflectedAt(line, i))
            {
                indices.Add(i);
            }
        }

        return indices;
    }

    private int GetHorizontalReflectionLine(string section)
    {
        return GetVerticalReflectionLine(TransposeSection(section));
    }

    private bool LineCanBeReflectedAt(string line, int index)
    {
        var (left, right) = line.SplitBefore(index);

        var minLength = Math.Min(left.Length, right.Length);

        return left.Reverse().LimitLength(minLength) == right.LimitLength(minLength);
    }

    private string TransposeSection(string section)
    {
        var grid = section.Grid();

        var builder = new StringBuilder();

        for (var c = 0; c < grid[0].Length; c++)
        {
            for (var r = 0; r < grid.Length; r++)
            {
                builder.Append(grid[r][c]);
            }
            builder.Append(Environment.NewLine);
        }

        return builder.ToString().Trim();
    }

    private int GetCorrectedHorizontalReflectionLine(string section)
    {
        return GetCorrectedVerticalReflectionLine(TransposeSection(section));

    }

    private int GetCorrectedVerticalReflectionLine(string section)
    {
        var lines = section.Lines();
        var indicesPerLine = lines.Select(GetReflectionIndicesForLine).ToList();

        // All but one line have an alternative index
        return Enumerable.Range(0, lines[0].Length)
            .SingleOrDefault(i => indicesPerLine
                            .Count(indices => indices.Contains(i)) == lines.Length - 1
                    );
    }
}
