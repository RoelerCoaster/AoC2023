namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class JaggedArrayGridExtensions
{
    public static T[][] CreateCopy<T>(this T[][] grid)
    {
        return grid.Select(x => x.ToArray()).ToArray();
    }

    public static string CreateString<T>(this T[][] grid)
    {
        return grid.Select(x => x.StringJoin("")).StringJoin(Environment.NewLine);
    }
}
