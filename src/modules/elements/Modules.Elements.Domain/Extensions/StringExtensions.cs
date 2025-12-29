using System.Numerics;
using System.Text.RegularExpressions;

namespace Starlights.Modules.Elements.Domain;

public static partial class StringExtensions
{
    extension(string value)
    {
        /// <summary>
        /// Determines whether the current value represents a valid numeric value.
        /// </summary>
        /// <remarks>This method considers leading and trailing whitespace when determining if the value
        /// is numeric. The definition of numeric is based on the ability to parse the value as a <see
        /// cref="System.Numerics.BigInteger"/>.</remarks>
        /// <returns><see langword="true"/> if the value can be parsed as a number; otherwise, <see langword="false"/>.</returns>
        public bool IsNumeric()
        {
            return BigInteger.TryParse(value.Trim(), out _);
        }

        /// <summary>
        /// Normalizes the string by trimming whitespace, converting to lowercase, and replacing spaces with hyphens.
        /// </summary>
        /// <remarks>
        /// Allowed characters are a-z, 0-9, hyphens (-), and colons (:). All other characters are replaced with hyphens.
        /// </remarks>
        public string NormalizeStatistic()
        {
            var normalizedValue = value.Trim().ToLowerInvariant();

            // remove straight and curly apostrophes in one pass
            normalizedValue = normalizedValue.Replace(' ', '-');
            normalizedValue = ApostrophesRegex().Replace(normalizedValue, string.Empty);
            normalizedValue = NormalizeRegex().Replace(normalizedValue, "-");
            normalizedValue = DuplicateHyphensRegex().Replace(normalizedValue, "-");
            normalizedValue = DuplicateColonsRegex().Replace(normalizedValue, ":");

            return normalizedValue.Trim('-', ':'); // trim any leading or trailing hyphens or colons
        }
    }

    [GeneratedRegex(@"[^a-z0-9-:]")]
    private static partial Regex NormalizeRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex DuplicateHyphensRegex();

    [GeneratedRegex(@"\:{2,}")]
    private static partial Regex DuplicateColonsRegex();

    [GeneratedRegex(@"['’]")]
    private static partial Regex ApostrophesRegex();
}

