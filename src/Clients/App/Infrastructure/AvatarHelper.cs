using System.Security.Cryptography;
using System.Text;
using System.Web;
using MudBlazor;

namespace Spenses.App.Infrastructure;

public static class AvatarHelper
{
    public enum GravatarDefaultType
    {
        /// <summary>
        /// Mystery person: a simple, cartoon-style silhouetted outline of a person (does not vary by email hash)
        /// </summary>
        Mp,

        /// <summary>
        /// A geometric pattern based on an email hash
        /// </summary>
        Identicon,

        /// <summary>
        /// A generated 'monster' with different colors, faces, etc.
        /// </summary>
        MonsterId,

        /// <summary>
        /// Generated faces with differing features and backgrounds.
        /// </summary>
        Wavatar,

        /// <summary>
        /// 8-bit arcade-style pixelated faces.
        /// </summary>
        Retro,

        /// <summary>
        /// A generated robot with different colors, faces, etc.
        /// </summary>
        Robohash,

        /// <summary>
        /// A transparent PNG image.
        /// </summary>
        Blank
    }

    /// <summary>
    /// Get a <see href="https://en.gravatar.com/site/implement">Gravatar</see> avatar URL for a given email
    /// address.
    /// </summary>
    /// <param name="emailAddress">The email address to get a Gravatar for.</param>
    /// <param name="size">The size of the avatar.</param>
    /// <param name="force">When true, forces the default image to always be loaded.</param>
    /// <param name="defaultTypeAvatar">
    /// The type of default avatar to use when no avatar exists for the supplied email.
    /// </param>
    /// <returns>A <see cref="Uri" /> for the requested Gravatar.</returns>
    public static Uri GetGravatarUri(string emailAddress, Size size = Size.Medium, bool force = false,
        GravatarDefaultType defaultTypeAvatar = GravatarDefaultType.Identicon)
    {
        if (string.IsNullOrEmpty(emailAddress))
            throw new ArgumentNullException(nameof(emailAddress));

        var emailHash = CalculateSha256Hash(emailAddress);

        var uriBuilder = new UriBuilder("https://www.gravatar.com") { Path = $"avatar/{emailHash}" };

        var queryString = HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("r", "pg");
        queryString.Add("default", defaultTypeAvatar.ToString().ToLowerInvariant());

        var sizeValue = size switch
        {
            Size.Small => 24,
            Size.Medium => 40,
            Size.Large => 56,
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        queryString.Add("s", sizeValue.ToString());

        if (force)
            queryString.Add("f", "y");

        uriBuilder.Query = queryString.ToString();

        return uriBuilder.Uri;
    }

    private static string CalculateSha256Hash(string input)
    {
        var bytes = Encoding.ASCII.GetBytes(input.ToLowerInvariant());

        var hash = SHA256.HashData(bytes);

        var sb = new StringBuilder();

        foreach (var t in hash)
            sb.Append($"{t:X2}");

        return sb.ToString().ToLower();
    }
}
