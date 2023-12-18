using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day18;

internal class Day18 : DayBase
{
    public override int Day => 18;

    public override bool UseTestInput => true;

    protected override PartToRun PartsToRun => PartToRun.Part2;

    protected override async Task<string> SolvePart1(string input)
    {
        var actions = input.Lines()
           .Select(l =>
           {
               var split = l.Split(" ");
               return (CharToDirection(split[0][0]), split[1].ToNumber<int>());
           })
           .ToArray();

        var edge = TraceEdge(actions);
        var area = GetArea(edge);

        return area.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var directionMap = new[] { 'R', 'D', 'L', 'U' };
        var actions = input.Lines()
           .Select(l =>
           {
               var split = l.Split(" ");

               var cleaned = split[2].Trim('#', '(', ')');

               var steps = Convert.ToInt32(cleaned.Substring(0, 5), 16);
               var direction = CharToDirection(directionMap[cleaned.Substring(5).ToNumber<int>()]);

               return (direction, steps);
           })
           .ToArray();

        var edge = TraceEdge(actions);
        var area = GetArea(edge);

        return area.ToString();
    }

    private List<GridCoordinate> TraceEdge((CardinalDirection Direction, int steps)[] actions)
    {
        var current = new GridCoordinate(0, 0);
        var edge = new List<GridCoordinate> { current };




        foreach (var (direction, steps) in actions)
        {
            for (var i = 0; i < steps; i++)
            {
                current = current.CoordinateInDirection(direction);
                edge.Add(current);
            }
        }

        return edge;
    }

    private int GetArea(List<GridCoordinate> edge)
    {
        var edgeSet = edge.ToHashSet();

        var minRow = edge.Min(e => e.Row);
        var maxRow = edge.Max(e => e.Row);
        var minCol = edge.Min(e => e.Col);
        var maxCol = edge.Max(e => e.Col);

        var total = 0;


        for (var r = minRow; r <= maxRow; r++)
        {
            var isInside = false;
            CardinalDirection? cornerDirectionStart = null;

            for (var c = minCol; c <= maxCol; c++)
            {
                var coordinate = new GridCoordinate(r, c);
                var onEdge = edgeSet.Contains(coordinate);
                if (isInside || onEdge)
                {
                    total++;
                }

                if (onEdge)
                {
                    var northSouth = new[] {
                        CardinalDirection.North,
                        CardinalDirection.South
                    }.Where(d => edgeSet.Contains(coordinate.CoordinateInDirection(d)))
                    .ToHashSet();

                    if (cornerDirectionStart is null)
                    {
                        if (northSouth.Count == 2)
                        {
                            // straight edge
                            isInside = !isInside;
                        }
                        else
                        {
                            // start corner
                            cornerDirectionStart = northSouth.Single();
                        }

                    }
                    else if (northSouth.Count == 1)
                    {
                        if (northSouth.Single() == cornerDirectionStart.Value.Flip())
                        {
                            // end corner going oposite direction. Treat as straight edge.
                            isInside = !isInside;
                        }
                        cornerDirectionStart = null;
                    }
                }
            }
        }

        return total;
    }

    private CardinalDirection CharToDirection(char c)
    {
        return c switch
        {
            'R' => CardinalDirection.East,
            'L' => CardinalDirection.West,
            'U' => CardinalDirection.North,
            'D' => CardinalDirection.South,
            _ => throw new NotSupportedException()
        };
    }
}
