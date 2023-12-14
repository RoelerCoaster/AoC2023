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

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> nullableElements)
    {
        return nullableElements.Where(e => e is not null)!;
    }

    public static IEnumerable<(T, T)> UnorderedPairs<T>(this IEnumerable<T> elements, bool skipSelf)
    {
        var list = elements.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            for (var j = i; j < list.Count; j++)
            {
                if (i == j && skipSelf)
                {
                    continue;
                }

                yield return (list[i], list[j]);
            }
        }
    }

    public static string StringJoin<T>(this IEnumerable<T> elements, string separator)
    {
        return string.Join(separator, elements);
    }
}
