namespace RoelerCoaster.AdventOfCode.Year2023.Internals.Model;

[Flags]
internal enum PartToRun
{
    None = 0,

    Part1 = 1,

    Part2 = 2,

    Both = Part1 | Part2
}
