namespace Starlights.Modules.Characters.Services.Statistics;

internal static class Extensions
{
    /// <summary>
    /// Converts the specified string to a URL-friendly slug by lowercasing, trimming, and replacing spaces with
    /// hyphens.
    /// </summary>
    /// <remarks>This method does not remove or replace special characters other than spaces. The resulting
    /// slug may still contain non-alphanumeric characters if present in the input.</remarks>
    /// <param name="str">The input string to convert to a slug. Cannot be null.</param>
    /// <returns>A slugified version of the input string, with all characters in lowercase, leading and trailing whitespace
    /// removed, and spaces replaced by hyphens.</returns>
    public static string ToSlug(this string str)
    {
        return str.ToLowerInvariant().Trim().Replace(' ', '-');
    }
}
