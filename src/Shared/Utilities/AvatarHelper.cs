using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Spenses.Shared.Utilities;

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

public static class AvatarHelper
{

    /// <summary>
    /// Get a <see href="https://en.gravatar.com/site/implement">Gravatar</see> avatar URL for a given email
    /// address.
    /// </summary>
    /// <param name="emailAddress">The email address to get a Gravatar for.</param>
    /// <param name="size">The pixel size of the avatar to request.</param>
    /// <param name="forceDefault">When true, forces the default image to always be loaded.</param>
    /// <param name="defaultAvatar">
    /// The type of default avatar to use when no avatar exists for the supplied email.
    /// </param>
    /// <returns>A <see cref="Uri" /> for the requested Gravatar.</returns>
    public static Uri GetGravatarUri(string emailAddress, int size = 80, bool forceDefault = false,
        GravatarDefaultType defaultAvatar = GravatarDefaultType.MonsterId)
    {
        var emailHash = CalculateSha256Hash(emailAddress);

        var uriBuilder = new UriBuilder("https://www.gravatar.com") { Path = $"avatar/{emailHash}" };

        var queryString = HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("rating", "pg");
        queryString.Add("default", defaultAvatar.ToString().ToLowerInvariant());

        queryString.Add("size", size.ToString());

        if (forceDefault)
            queryString.Add("forcedefault", "y");

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
