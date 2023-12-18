using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day17;

internal class Day17 : DayBase
{
    public override int Day => 17;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Lines().Select(l => l.Digits()).ToArray();

        var result = MinHeatLoss(grid, new(0, 0), new(grid.Length - 1, grid[0].Length - 1), 3);

        return result.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    // Basicallly a modified A*
    private int MinHeatLoss(int[][] grid, GridCoordinate start, GridCoordinate dest, int maxSteps)
    {
        var crucibleStart = new CrucibleData(start, maxSteps, CardinalDirection.East);
        var openSet = new HashSet<CrucibleData> { crucibleStart };

        var cellData = new Dictionary<CrucibleData, CellData>(grid.Length * grid[0].Length * maxSteps * 4);
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                for (var i = 0; i < maxSteps; i++)
                {
                    foreach (var direction in Enum.GetValues<CardinalDirection>())
                    {
                        cellData[new(r, c, i, direction)] = new();
                    }
                }
            }
        }

        cellData[crucibleStart] = new() { GScore = 0 };

        while (openSet.Count > 0)
        {
            var current = openSet.MinBy(c => cellData[c].FScore)!;
            var currentCellData = cellData[current];

            if (new GridCoordinate(current.Row, current.Col) == dest)
            {
                return currentCellData.GScore;
            }

            openSet.Remove(current);

            foreach (var neighbor in GetNeighbors(current, maxSteps, grid.Length - 1, grid[0].Length - 1))
            {
                var neighborCellData = cellData[neighbor];
                var gScore = currentCellData.GScore + grid[neighbor.Row][neighbor.Col];
                if (gScore < neighborCellData.GScore)
                {
                    neighborCellData.Parent = current;
                    neighborCellData.GScore = gScore;
                    // Manhattan distance as heuristic
                    neighborCellData.FScore = gScore + Math.Abs(dest.Row - neighbor.Row) + Math.Abs(dest.Col - neighbor.Col);

                    openSet.Add(neighbor);
                }
            }
        }

        throw new Exception("No path found");
    }

    private IEnumerable<CrucibleData> GetNeighbors(CrucibleData current, int maxSteps, int maxRow, int maxCol)
    {
        var potentialCoordinates = new List<CrucibleData>
        {
            new (CoordinateInDirection(current, current.Direction.RotateClockwise()), maxSteps - 1, current.Direction.RotateClockwise()),
            new (CoordinateInDirection(current, current.Direction.RotateCounterClockwise()), maxSteps - 1, current.Direction.RotateCounterClockwise()),
            new (CoordinateInDirection(current, current.Direction), current.StepsRemaining - 1, current.Direction)
        };

        foreach (var coordinate in potentialCoordinates)
        {
            // Coordinate must be inside the grid
            if (coordinate.Row < 0 || coordinate.Col < 0 || coordinate.Row > maxRow || coordinate.Col > maxCol || coordinate.StepsRemaining < 0)
            {
                continue;
            }

            yield return coordinate;

        }
    }

    private GridCoordinate CoordinateInDirection(GridCoordinate current, CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => current with { Row = current.Row - 1 },
            CardinalDirection.South => current with { Row = current.Row + 1 },
            CardinalDirection.West => current with { Col = current.Col - 1 },
            CardinalDirection.East => current with { Col = current.Col + 1 },
            _ => throw new NotSupportedException()
        }; ;
    }


    private void PrintPath(int[][] grid, CrucibleData dest, Dictionary<CrucibleData, CellData> cellData)
    {
        var copy = grid.Select(x => x.Select(y => y.ToString()).ToArray()).ToArray();

        var current = dest;
        while (current is not null)
        {
            copy[current.Row][current.Col] = ".";
            current = cellData[current].Parent;
        }

        var print = copy.CreateString();

        AnsiConsole.WriteLine(print);
    }
}

internal class CellData
{
    public CrucibleData? Parent { get; set; }
    public int GScore { get; set; } = int.MaxValue;
    public int FScore { get; set; } = int.MaxValue;
}

internal record CrucibleData(int Row, int Col, int StepsRemaining, CardinalDirection Direction) : GridCoordinate(Row, Col)
{
    public CrucibleData(GridCoordinate coordinate, int stepsRemaining, CardinalDirection direction) : this(coordinate.Row, coordinate.Col, stepsRemaining, direction)
    {

    }
};
