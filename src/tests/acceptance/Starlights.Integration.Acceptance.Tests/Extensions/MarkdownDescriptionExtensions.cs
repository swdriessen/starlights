using Starlights.Integration.Acceptance.Tests.StepDefinitions.Helpers;

namespace Starlights.Integration.Acceptance.Tests.Extensions;

internal static class MarkdownDescriptionExtensions
{
    extension(ScenarioContext context)
    {
        internal static string MarkdownDescriptionKey => "<markdown description>";

        internal void SetMarkdownDescription(string description)
        {
            context[ScenarioContext.MarkdownDescriptionKey] = description;
        }

        internal string GetMarkdownDescription()
        {
            return context.Get<string>(ScenarioContext.MarkdownDescriptionKey);
        }
    }

    extension(CreateSpellTableRow row)
    {
        internal CreateSpellTableRow WithMarkdownDescription(ScenarioContext context)
        {
            return row with { Description = row.Description.ReplaceWithMarkdownDescription(context) };

        }
    }

    extension(UpdateSpellTableRow row)
    {
        internal UpdateSpellTableRow WithMarkdownDescription(ScenarioContext context)
        {
            return row.Description is null ? row : (row with { Description = row.Description.ReplaceWithMarkdownDescription(context) });
        }
    }

    extension(string value)
    {
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
}