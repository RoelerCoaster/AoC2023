using System.Numerics;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class MathUtil
{
    public static T GCD<T>(T a, T b) where T : IBinaryNumber<T>
    {
        while (b != T.Zero)
        {
            (a, b) = (b, a % b);
        };

        return a;
    }

    public static T Mod<T>(T a, T b) where T : IBinaryInteger<T>
    {
        return (a % b + b) % b;
    }

    public static (T GCD, T BezoutS, T BezoutT) ExtendedGCD<T>(T a, T b) where T : IBinaryInteger<T>
    {
        var (oldR, r) = (a, b);
        var (oldS, s) = (T.One, T.Zero);
        var (oldT, t) = (T.Zero, T.One);

        while (r != T.Zero)
        {
            var (quot, rem) = T.DivRem(oldR, r);
            (oldR, r) = (r, rem);
            (oldS, s) = (s, oldS - quot * s);
            (oldT, t) = (t, oldT - quot * t);
        }

        return (oldR, oldS, oldT);
    }

    public static T LCM<T>(T a, T b) where T : IBinaryNumber<T>
    {
        return T.CopySign(a * b, T.One) / GCD(a, b);
    }
}
