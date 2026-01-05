using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Starlights.Platform.Components.Data.EntityFramework.Extensions;

/// <summary>
/// Provides helpers for configuring JSON value object conversions on EF Core property builders.
/// </summary>
public static class PropertyBuilderJsonExtensions
{
    /// <summary>
    /// Configures the property to use the <see cref="JsonValueObjectConverter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder for chaining.</returns>
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasConversion(JsonValueObjectConverter<T>.Default);

        return builder;
    }
}
