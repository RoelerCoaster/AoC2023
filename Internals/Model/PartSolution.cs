namespace RoelerCoaster.AdventOfCode.Year2023.Internals.Model;

internal class PartSolution
{
    public string? Answer { get; private init; }
    public TimeSpan? Elapsed { get; private init; }

    public SolutionType Type { get; private init; }

    public Exception? Exception { get; private init; }

    public static PartSolution Valid(string? answer, TimeSpan elapsed)
    {
        return new PartSolution
        {
            Answer = answer,
            Elapsed = elapsed,
            Type = SolutionType.Valid
        };
    }

    public static PartSolution Error(Exception ex)
    {
        return new PartSolution
        {
            Exception = ex,
            Type = SolutionType.Error
        };
    }

    public static PartSolution Skipped()
    {
        return new PartSolution
        {
            Type = SolutionType.Skipped
        };
    }
}
