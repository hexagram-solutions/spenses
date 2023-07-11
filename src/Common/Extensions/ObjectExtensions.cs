namespace Spenses.Common.Extensions;

/// <summary>
/// Extensions for arbitrary <see cref="object"/>s.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Yield the object as an <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns> An <see cref="IEnumerable{T}" /> containing the item.</returns>
    public static IEnumerable<T> Yield<T>(this T? item)
    {
        if (item is null)
            yield break;

        yield return item;
    }

    /// <summary>
    /// Returns the value for the selected property if the item is not null, otherwise returns the default value for
    /// the property type.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <typeparam name="TMember">The type of selected property.</typeparam>
    /// <param name="item">The object to select from.</param>
    /// <param name="selector">The property selector.</param>
    /// <returns>
    /// The value for the member if not null, otherwise the default value for <typeparamref name="TMember"/>
    /// </returns>
    /// <remarks>
    /// In the <b>vast majority</b> of cases, a null-coalescing operator (<c>??</c> or <c>??=</c>) should be used
    /// instead of this method. This method is useful in places those aren't allowed, such as expression tree lambdas.
    /// </remarks>
    public static TMember? ValueOrDefault<T, TMember>(this T? item, Func<T?, TMember?> selector)
    {
        return item is null ? default : selector(item);
    }
}
