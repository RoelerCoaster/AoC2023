using RoelerCoaster.AdventOfCode.Year2023.Util.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class CardinalDirectionExtensions
{
    public static CardinalDirection RotateClockwise(this CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => CardinalDirection.East,
            CardinalDirection.East => CardinalDirection.South,
            CardinalDirection.South => CardinalDirection.West,
            CardinalDirection.West => CardinalDirection.North,
            _ => throw new NotSupportedException(),
        };
    }

    public static CardinalDirection RotateCounterClockwise(this CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => CardinalDirection.West,
            CardinalDirection.East => CardinalDirection.North,
            CardinalDirection.South => CardinalDirection.East,
            CardinalDirection.West => CardinalDirection.South,
            _ => throw new NotSupportedException(),

        };
    }

    public static CardinalDirection Flip(this CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.North => CardinalDirection.South,
            CardinalDirection.East => CardinalDirection.West,
            CardinalDirection.South => CardinalDirection.North,
            CardinalDirection.West => CardinalDirection.East,
            _ => throw new NotSupportedException(),
        };
    }

}
