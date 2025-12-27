using System.Globalization;
using Starlights.Integration.Acceptance.Tests.Extensions;

namespace Starlights.Integration.Acceptance.Tests.Extensions;

internal static class DataTableExtensions
{
    extension(ScenarioContext context)
    {
        internal static string MarkdownDescriptionKey => "<markdown description>";

        /// <summary>
        /// Sets the markdown description in the scenario context.
        /// </summary>
        /// <param name="description">The markdown description.</param>
        /// <param name="key">The key under which to store the description. When omitted, a default key is used.</param>
        internal void SetMarkdownDescription(string description, string? key = null)
        {
            context[key ?? ScenarioContext.MarkdownDescriptionKey] = description;
        }

        /// <summary>
        /// Gets the markdown description from the scenario context.
        /// </summary>
        /// <param name="key">The key under which the description is stored. When omitted, a default key is used.</param>
        /// <returns>The markdown description.</returns>
        internal string GetMarkdownDescription(string? key = null)
        {
            return context.Get<string>(key ?? ScenarioContext.MarkdownDescriptionKey);
        }
    }

    extension<T>(DataTable table) where T : class, ITableRow
    {
        /// <summary>
        /// Creates an instance of T from the DataTable, replacing the markdown description if applicable.
        /// </summary>
        internal T CreateInstance(ScenarioContext context)
        {
            var instance = table.CreateInstance<T>();

            if (instance is IMarkdownDescriptionTableRow row && row.Description is not null)
            {
                row.Description = row.Description.ReplaceWithMarkdownDescription(context);
            }

            // store the instance in the scenario context for later use
            context.Set(instance, instance.GetType().FullName);

            return instance;
        }

        /// <summary>
        /// Creates a set of instances of T from the DataTable, replacing the markdown description if applicable.
        /// </summary>
        internal IEnumerable<T> CreateSet(ScenarioContext context)
        {
            var instanceSet = table.CreateSet<T>();

            foreach (var instance in instanceSet)
            {
                if (instance is IMarkdownDescriptionTableRow row && row.Description is not null)
                {
                    row.Description = row.Description.ReplaceWithMarkdownDescription(context);
                }

                // store the instance in the scenario context for later use
                context.Set(instance, instance.GetType().FullName);
            }

            return instanceSet;
        }
    }

    extension(string value)
    {
        /// <summary>
        /// Replaces the placeholder with the actual markdown description from the scenario context.
        /// </summary>
        internal string ReplaceWithMarkdownDescription(ScenarioContext context)
        {
            if (value == ScenarioContext.MarkdownDescriptionKey)
            {
                if (!context.ContainsKey(ScenarioContext.MarkdownDescriptionKey))
                {
                    throw new InvalidOperationException("the markdown description was not replaced because it is not available in the ScenarioContext");
                }

                value = context.GetMarkdownDescription();
                return value;
            }

            return value;
        }
    }

    internal static void AssertProvidedProperties<TExpected, TActual>(
        this DataTable dataTable,
        TExpected expected,
        TActual actual,
        IReadOnlyDictionary<string, Action<TExpected, TActual>> assertions)
    {
        var provided = dataTable.Header
            .Select(h => NormalizeHeader(h))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var header in provided)
        {
            if (assertions.TryGetValue(header, out var assert))
            {
                assert(expected, actual);
                continue;
            }

            throw new NotImplementedException($"checking property '{header}' is not implemented");
        }
    }

    private static string NormalizeHeader(string header)
    {
        return header.Trim().ToLower(CultureInfo.InvariantCulture);
    }
}