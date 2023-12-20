using RoelerCoaster.AdventOfCode.Year2023.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class GridCoordinateExtensions
{
    public static GridCoordinate CoordinateInDirection(this GridCoordinate current, CardinalDirection direction, int offset = 1)
    {
        return direction switch
        {
            CardinalDirection.North => current with { Row = current.Row - offset },
            CardinalDirection.South => current with { Row = current.Row + offset },
            CardinalDirection.West => current with { Col = current.Col - offset },
            CardinalDirection.East => current with { Col = current.Col + offset },
            _ => throw new NotSupportedException()
        }; ;
    }
}
