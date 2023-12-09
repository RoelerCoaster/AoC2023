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

    public static T LCM<T>(T a, T b) where T : IBinaryNumber<T>
    {
        return T.CopySign(a * b, T.One) / GCD(a, b);
    }
}
