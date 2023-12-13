using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;

public static class StringExtensions
{

    public static string[] Lines(this string s, bool removeEmptyLines = false)
    {
        var splitOptions = StringSplitOptions.TrimEntries;
        if (removeEmptyLines)
        {
            splitOptions |= StringSplitOptions.RemoveEmptyEntries;
        }

        return s.Split(Environment.NewLine, splitOptions);
    }

    public static TNumber[] NumberLines<TNumber>(this string s)
        where TNumber : INumber<TNumber>, IParsable<TNumber>
    {
        return s.Lines().Select(l => l.ToNumber<TNumber>()).ToArray();
    }

    public static string[] Sections(this string s)
    {
        return s.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    public static TNumber ToNumber<TNumber>(this string s)
        where TNumber : INumber<TNumber>, IParsable<TNumber>
    {
        return TNumber.Parse(s, NumberFormatInfo.InvariantInfo);
    }

    public static int[] Digits(this string s)
    {
        return s.Select(s => s.ToString().ToNumber<int>()).ToArray();
    }

    public static TNumber[] NumbersBySeparator<TNumber>(this string s, string separator)
        where TNumber : INumber<TNumber>, IParsable<TNumber>
    {
        return s.Split(separator).Select(n => n.ToNumber<TNumber>()).ToArray();
    }

    public static TNumber[] NumbersByRegex<TNumber>(this string s, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        where TNumber : INumber<TNumber>, IParsable<TNumber>
    {
        return Regex.Split(s, pattern).Select(n => n.ToNumber<TNumber>()).ToArray();
    }

    public static string SliceLines(this string s, int start, int? end)
    {
        var lines = s.Lines();
        return string.Join(Environment.NewLine, s.Lines()[start..(end ?? lines.Length)]);
    }

    public static char[][] Grid(this string s)
    {
        return s.Lines().Select(l => l.ToCharArray()).ToArray();
    }

    /// <summary>
    /// Splits the string right before the given index. This means s[index] will be the 
    /// first character of the Right result;
    /// </summary>
    public static (string Left, string Right) SplitBefore(this string s, int index)
    {
        return (s.Substring(0, index), s.Substring(index));
    }

    public static string LimitLength(this string s, int maxLength)
    {
        return s.Length > maxLength
            ? s.Substring(0, maxLength)
            : s;
    }

    public static string Reverse(this string s)
    {
        var array = s.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }
}
