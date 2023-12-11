using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day10;


internal class Day10 : DayBase
{
    public override int Day => 10;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();
        var (adjacencyList, start) = ParseGraph(grid);
        var loop = GetLoop(start, adjacencyList);
        var furthest = loop.Count / 2;

        return furthest.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();
        var (adjacencyList, start) = ParseGraph(grid);
        var loop = GetLoop(start, adjacencyList);

        CleanGrid(grid, loop);

        var print = string.Join('\n', grid.Select(x => string.Join("", x)));

        MarkGrid(grid);

        print = string.Join('\n', grid.Select(x => string.Join("", x)));

        return grid.SelectMany(x => x).Count(s => s == 'I').ToString();
    }

    private List<GridCoordinate> GetLoop(GridCoordinate start, Dictionary<GridCoordinate, HashSet<GridCoordinate>> adjacencyList)
    {
        var nextFromStart = adjacencyList[start].First();
        var loop = new List<GridCoordinate> { start, nextFromStart };
        while (loop[^1] != start)
        {
            var next = adjacencyList[loop[^1]].Single(l => l != loop[^2]);
            loop.Add(next);
        }

        return loop;
    }

    private (Dictionary<GridCoordinate, HashSet<GridCoordinate>> adjacencyList, GridCoordinate start) ParseGraph(char[][] grid)
    {

        var adjacencyList = new Dictionary<GridCoordinate, HashSet<GridCoordinate>>();
        GridCoordinate? start = null;

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                var currentCoordinate = new GridCoordinate(row, col);
                HashSet<GridCoordinate>? adjacent = grid[row][col] switch
                {
                    '|' => [new GridCoordinate(row - 1, col), new GridCoordinate(row + 1, col)],
                    '-' => [new GridCoordinate(row, col - 1), new GridCoordinate(row, col + 1)],
                    'L' => [new GridCoordinate(row - 1, col), new GridCoordinate(row, col + 1)],
                    'J' => [new GridCoordinate(row - 1, col), new GridCoordinate(row, col - 1)],
                    '7' => [new GridCoordinate(row + 1, col), new GridCoordinate(row, col - 1)],
                    'F' => [new GridCoordinate(row + 1, col), new GridCoordinate(row, col + 1)],
                    _ => null
                };

                if (grid[row][col] == 'S')
                {
                    // Adjacency to be filled later
                    start = currentCoordinate;
                }

                if (adjacent != null)
                {
                    adjacencyList[currentCoordinate] = adjacent;
                }
            }
        }

        if (start == null)
        {
            throw new InvalidOperationException("Start not found");
        }

        // Fill the adjacency of the start coordinate
        adjacencyList[start] = adjacencyList.Where(kv => kv.Value.Contains(start)).Select(kv => kv.Key).ToHashSet();


        return (adjacencyList, start);
    }

    private void CleanGrid(char[][] grid, List<GridCoordinate> loop)
    {
        var startType = GetStartType(loop[0], [loop[^2], loop[1]]);

        var loopSet = loop.ToHashSet();

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                var coordinate = new GridCoordinate(row, col);
                if (!loopSet.Contains(coordinate))
                {
                    grid[row][col] = '.';
                }

                if (coordinate == loop[0])
                {
                    grid[row][col] = startType;
                }
            }
        }
    }

    private char GetStartType(GridCoordinate start, List<GridCoordinate> adjacent)
    {
        var north = adjacent.Any(a => a.Row == start.Row - 1);
        var south = adjacent.Any(a => a.Row == start.Row + 1);
        var east = adjacent.Any(a => a.Col == start.Col - 1);
        var west = adjacent.Any(a => a.Col == start.Col + 1);

        if (north && south)
        {
            return '|';
        }

        if (east && west)
        {
            return '-';
        }

        if (north && east)
        {
            return 'J';
        }

        if (north && west)
        {
            return 'L';
        }

        if (south && east)
        {
            return '7';
        }

        if (south && west)
        {
            return 'F';
        }

        throw new InvalidOperationException("unknown start symbol");
    }

    private void MarkGrid(char[][] grid)
    {
        var scanLine = grid[0].Select(_ => new MarkState()).ToArray();

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                var currentMarkState = scanLine[col];

                switch (grid[row][col])
                {
                    case '.':
                        grid[row][col] = currentMarkState.Mark;
                        break;
                    case '-':
                        currentMarkState.FlipMark();
                        break;
                    case '|' when !currentMarkState.IsBending:
                        throw new InvalidOperationException("Vertical pipe when not bending.");
                    case 'F':
                    case '7':
                        currentMarkState.StartBend(grid[row][col]);
                        break;
                    case 'J':
                    case 'L':
                        currentMarkState.EndBend(grid[row][col]);
                        break;
                }
            }
        }
    }
}

internal class MarkState
{
    public char Mark { get; private set; } = 'O';

    public bool IsBending => _bendMark != null;

    private char? _bendMark = null;

    public void FlipMark()
    {
        Mark = Mark == 'O' ? 'I' : 'O';
    }

    public void StartBend(char bendMark)
    {
        if (bendMark != 'F' && bendMark != '7')
        {
            throw new InvalidOperationException("Invalid start of bend");
        }

        _bendMark = bendMark;
        FlipMark();
    }

    public void EndBend(char bendMark)
    {
        if (bendMark != 'J' && bendMark != 'L')
        {
            throw new InvalidOperationException("Invalid end of bend");
        }

        if ((_bendMark == '7' && bendMark == 'J') || (_bendMark == 'F' && bendMark == 'L'))
        {
            FlipMark();
        }

        _bendMark = null;
    }
}

