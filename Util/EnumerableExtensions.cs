using System.Numerics;

namespace RoelerCoaster.AdventOfCode.Year2023.Util;
internal static class EnumerableExtensions
{
    public static TNumber Product<TNumber>(this IEnumerable<TNumber> numbers) where TNumber : INumber<TNumber>
    {
        return numbers.Product(n => n);
    }

    public static TNumber Product<TNumber, TElement>(this IEnumerable<TElement> elements, Func<TElement, TNumber> numberSelector) where TNumber : INumber<TNumber>
    {
        return elements.Aggregate(TNumber.One, (acc, val) => acc * numberSelector(val));
    }
}
