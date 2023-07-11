using System.Globalization;
using System.Text;

namespace Spenses.Common.Extensions;

/// <summary>
/// Extension methods for working with <see cref="string"/>s.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to lowerCamelCase.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>The converted string.</returns>
    public static string ToLowerCamelCase(this string s)
    {
        return s.ToCamelCase();
    }

    /// <summary>
    /// Converts a string to UpperCamelCase.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>The converted string.</returns>
    public static string ToUpperCamelCase(this string s)
    {
        return s.ToCamelCase(true);
    }

    /// <summary>
    /// Converts a string to camelCase or UpperCamelCase, stripping any diacritics.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="upperCamelCase">True to convert to UpperCamelCase, otherwise convert to lowerCamelCase.</param>
    /// <returns>The converted string.</returns>
    private static string ToCamelCase(this string s, bool upperCamelCase = false)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        // This splits letters with diacritics into separate characters so we can strip them away
        s = s.Normalize(NormalizationForm.FormD);

        int i = 0, j = 0;

        var isStartOfWord = upperCamelCase;

        var result = new char[s.Length];

        // Skip any whitespace, punctuation, etc. in front of the first word...
        while (i < s.Length && !char.IsLetterOrDigit(s[i]))
            i++;

        for (; i < s.Length; i++)
        {
            // Anything that isn't a letter is the start of a new word...
            if (!char.IsLetter(s[i]))
            {
                // ... except in the edge case of the apostrophe in a contraction
                if ((s[i] == '\'' || s[i] == '\u2019') && 0 < i && i < s.Length - 1 && char.IsLetter(s[i - 1]) &&
                    char.IsLetter(s[i + 1]))
                    continue;

                // ... or a diacritic to be applied to another letter
                if (CharUnicodeInfo.GetUnicodeCategory(s[i]) == UnicodeCategory.NonSpacingMark)
                    continue;

                isStartOfWord = true;

                // We'll keep digits in the string...
                if (char.IsDigit(s[i]))
                    result[j++] = s[i];

                // But anything else that's both a non-letter and a non-digit gets skipped
                continue;
            }

            // We treat a translation from a uppercase to lowercase as the start of a new word. This handles the case
            // of applying ToCamelCase to a string that is already camel-cased.
            if (i > 0 && char.IsLower(s[i - 1]) && char.IsUpper(s[i]))
                isStartOfWord = true;

            if (isStartOfWord)
            {
                result[j++] = char.ToUpperInvariant(s[i]);
                isStartOfWord = false;
            }
            else
            {
                result[j++] = char.ToLowerInvariant(s[i]);
            }
        }

        return new string(result, 0, j);
    }

    /// <summary>
    /// Replace the diacritic characters in in a string with their ASCII equivalents when possible. For example:
    /// <example>
    ///     <code>"Hafþór Júlíus Björnsson".StripDiacritics() == "Hafthor Julius Bjornsson"</code>
    /// </example>
    /// </summary>
    /// <param name="value">The value with diacritics.</param>
    /// <returns>The value without diacritics.</returns>
    /// <see href="http://stackoverflow.com/a/249126/1672990" />
    public static string StripDiacritics(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var character in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(character);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Converts the string to a valid file name by replacing invalid chars with underscores or a given value.
    /// (e.g. <c>"08/03/2017".ToValidFileName() == "08_03_2017"</c>)
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="replacement">The string to replace invalid characters with.</param>
    /// <returns>A valid filename.</returns>
    public static string ToValidFileName(this string value, string replacement = "_")
    {
        var invalidChars = Path.GetInvalidFileNameChars();

        if (replacement.ToCharArray().Intersect(invalidChars).Any())
        {
            throw new ArgumentException($"{nameof(replacement)} cannot contain invalid file name characters.",
                nameof(replacement));
        }

        var result = new StringBuilder();

        foreach (var c in value.StripDiacritics())
        {
            if (c is >= ' ' and <= '~' && Array.IndexOf(invalidChars, c) < 0)
                result.Append(c);
            else
                result.Append(replacement);
        }

        return result.ToString();
    }
}
