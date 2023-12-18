using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class GridCoordinateExtensions
{
    public static GridCoordinate CoordinateInDirection(this GridCoordinate current, CardinalDirection direction)
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
}
