namespace Starlights.RuleSets.DND5E5.Content;

/// <summary>
/// Defines an input statistic and its supported modifier expression.
/// </summary>
public sealed record AbilityProperty(string Prefix, string Modifier, int? DefaultInput = null, int Maximum = 20) : IElementProperty;
