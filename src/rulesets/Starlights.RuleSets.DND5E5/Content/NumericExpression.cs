using System.Globalization;
using System.Text.RegularExpressions;

namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// A bounded arithmetic interpreter supporting numeric literals, statistic references and floor.
/// </summary>
public sealed class NumericExpression
{

    /// <summary>
    /// Reads declared statistic dependencies for content validation.
    /// </summary>
    public static IReadOnlyList<string> References(string text)
    {
        _ = Parse(text);
        return Regex.Matches(text, @"[a-z][a-z0-9-]*(?:\.[a-z][a-z0-9-]*)*")
            .Select(match => match.Value).Where(value => value != "floor").Distinct(StringComparer.Ordinal).ToArray();
    }
    private readonly Func<Func<string, double?>, double?> _evaluate;
    private NumericExpression(Func<Func<string, double?>, double?> evaluate)
    {
        _evaluate = evaluate;
    }

    /// <summary>
    /// Evaluates dependencies without replacing unresolved inputs with zero.
    /// </summary>
    public int? Evaluate(Func<string, double?> statistic)
    {
        var value = _evaluate(statistic);
        return value.HasValue && double.IsFinite(value.Value) && value.Value is >= -100000 and <= 100000
            ? checked((int)Math.Floor(value.Value)) : null;
    }

    /// <summary>
    /// Parses only the supported arithmetic vocabulary; no scripts or runtime compilation.
    /// </summary>
    public static NumericExpression Parse(string text)
    {
        if (text.Length is 0 or > 256)
        {
            throw new InvalidOperationException("Unsupported numeric expression.");
        }

        var tokens = Regex.Matches(text, @"floor|[a-z][a-z0-9-]*(?:\.[a-z][a-z0-9-]*)*|[0-9]+|[()+*/-]");
        if (string.Concat(tokens.Select(token => token.Value)) != Regex.Replace(text, @"\s", ""))
        {
            throw new InvalidOperationException("Unsupported numeric expression.");
        }

        var index = 0;
        string? Peek()
        {
            return index < tokens.Count ? tokens[index].Value : null;
        }

        void Take(string expected)
        {
            if (Peek() != expected)
            {
                throw new InvalidOperationException("Invalid numeric expression.");
            }

            index++;
        }
        Func<Func<string, double?>, double?> Factor(int depth)
        {
            if (depth > 20)
            {
                throw new InvalidOperationException("Numeric expression is too deeply nested.");
            }

            var token = Peek() ?? throw new InvalidOperationException("Incomplete numeric expression.");
            index++;
            if (token == "floor")
            {
                Take("("); var inside = Sum(depth + 1); Take(")");
                return read => inside(read) is { } value ? Math.Floor(value) : null;
            }
            if (token == "(")
            {
                var inside = Sum(depth + 1);
                Take(")");
                return inside;
            }

            if (token == "-")
            {
                var inside = Factor(depth + 1);
                return read => -inside(read);
            }
            if (double.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
            {
                return _ => number;
            }

            if (!Regex.IsMatch(token, @"^[a-z]"))
            {
                throw new InvalidOperationException("Invalid numeric expression.");
            }

            return read => read(token);
        }
        Func<Func<string, double?>, double?> Product(int depth)
        {
            var left = Factor(depth);
            while (Peek() is "*" or "/")
            {
                var operation = Peek(); index++;
                var earlier = left; var right = Factor(depth);
                left = read => operation == "*" ? earlier(read) * right(read) : right(read) is { } divisor && divisor != 0 ? earlier(read) / divisor : null;
            }
            return left;
        }
        Func<Func<string, double?>, double?> Sum(int depth)
        {
            var left = Product(depth);
            while (Peek() is "+" or "-")
            {
                var operation = Peek(); index++;
                var earlier = left; var right = Product(depth);
                left = read => operation == "+" ? earlier(read) + right(read) : earlier(read) - right(read);
            }
            return left;
        }
        var expression = Sum(0);
        if (index != tokens.Count)
        {
            throw new InvalidOperationException("Invalid numeric expression.");
        }

        return new(expression);
    }
}
