namespace Spenses.Common.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        var iEnumerable = source as T[] ?? source.ToArray();

        foreach (var item in iEnumerable)
            action(item);

        return iEnumerable;
    }

    public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        return Task.WhenAll(source.Select(action));
    }
}
