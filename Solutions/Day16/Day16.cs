using RoelerCoaster.AdventOfCode.Year2023.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2023.Util;
using RoelerCoaster.AdventOfCode.Year2023.Util.Model;
using System.Diagnostics.CodeAnalysis;

namespace RoelerCoaster.AdventOfCode.Year2023.Solutions.Day16;

internal class Day16 : DayBase
{
    public override int Day => 16;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();
        var cells = GetEnergizedCells(grid, new BeamState(0, -1, CardinalDirection.East));

        return cells.Count.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    private HashSet<GridCoordinate> GetEnergizedCells(char[][] grid, BeamState initialBeam)
    {
        var beams = new List<BeamState> { initialBeam };
        var encounteredStates = new HashSet<(int, int, CardinalDirection)>();
        var energizedCells = new HashSet<GridCoordinate>();

        while (beams.Count != 0)
        {
            // first move all beams forward
            beams.ForEach(b => b.Move());

            // Remove all invalid
            beams.RemoveAll(b => !b.IsInGrid(grid));

            // loop detection
            beams.RemoveAll(b => encounteredStates.Contains(b.StateTuple));

            var beamsToAdd = new List<BeamState>();
            // Handle grid cell
            foreach (var beam in beams.AsEnumerable())
            {
                energizedCells.Add(new GridCoordinate(beam.Row, beam.Col));
                encounteredStates.Add(beam.StateTuple);
                var cell = grid[beam.Row][beam.Col];

                if (cell is '/' or '\\')
                {
                    beam.Reflect(cell);
                }

                if (cell is '|' or '-' && beam.TrySplit(cell, out var additionalBeam))
                {
                    beamsToAdd.Add(additionalBeam);
                }
            }

            beams.AddRange(beamsToAdd);
        }

        return energizedCells;
    }


}

internal class BeamState
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public CardinalDirection Direction { get; private set; }

    public BeamState(int row, int col, CardinalDirection direction)
    {
        Row = row;
        Col = col;
        Direction = direction;
    }

    public void Move()
    {
        switch (Direction)
        {
            case CardinalDirection.North:
                Row--;
                break;
            case CardinalDirection.South:
                Row++;
                break;
            case CardinalDirection.West:
                Col--;
                break;
            case CardinalDirection.East:
                Col++;
                break;
        }
    }

    public void Reflect(char mirror)
    {
        if (mirror is '/')
        {
            if (Direction is CardinalDirection.East or CardinalDirection.West)
            {
                Direction = Direction.RotateCounterClockwise();
            }
            else
            {
                Direction = Direction.RotateClockwise();
            }
        }
        else if (mirror is '\\')
        {
            if (Direction is CardinalDirection.East or CardinalDirection.West)
            {
                Direction = Direction.RotateClockwise();
            }
            else
            {
                Direction = Direction.RotateCounterClockwise();
            }
        }
        else
        {
            throw new InvalidOperationException("Invalid mirror.");
        }
    }

    public bool TrySplit(char splitter, [MaybeNullWhen(false)] out BeamState additionalBeam)
    {
        if (splitter is '|' && Direction is CardinalDirection.East or CardinalDirection.West)
        {
            Direction = CardinalDirection.North;
            additionalBeam = new BeamState(Row, Col, CardinalDirection.South);
            return true;
        }
        else if (splitter is '-' && Direction is CardinalDirection.North or CardinalDirection.South)
        {
            Direction = CardinalDirection.East;
            additionalBeam = new BeamState(Row, Col, CardinalDirection.West);
            return true;
        }
        additionalBeam = default;
        return false;
    }

    public bool IsInGrid(char[][] grid)
    {
        return Row >= 0 && Col >= 0 && Row < grid.Length && Col < grid[0].Length;
    }

    public (int, int, CardinalDirection) StateTuple => (Row, Col, Direction);
}