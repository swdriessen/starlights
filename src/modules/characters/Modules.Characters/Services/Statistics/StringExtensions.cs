namespace Starlights.Modules.Characters.Services.Statistics;

internal static class StringExtensions
{
    /// <summary>
    /// Converts the specified string to a statistic-friendly slug by lowercasing, trimming, and replacing spaces with hyphens.
    /// </summary>
    /// <param name="str">The input string to convert to a slug. Cannot be null.</param>
    /// <returns>A slugified version of the input string, with all characters in lowercase, leading and trailing whitespace
    /// removed, and spaces replaced by hyphens.</returns>
    public static string ToSlug(this string str)
    {
        return str.ToLowerInvariant().Trim().Replace(' ', '-');
    }
}
