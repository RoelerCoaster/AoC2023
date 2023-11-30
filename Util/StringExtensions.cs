﻿using System.Globalization;
using System.Numerics;

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

    public static string SliceLines(this string s, int start, int? end)
    {
        var lines = s.Lines();
        return string.Join(Environment.NewLine, s.Lines()[start..(end ?? lines.Length)]);
    }
}