using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Characters.Endpoints.Entities.AbilityScores;
using Starlights.Modules.Characters.Endpoints.Entities.Skills;

namespace Starlights.Modules.Characters.Endpoints.Extensions;

public static class DataModelMappingExtensions
{
    public static AbilityScoreDataModel AsAbilityScoreDataModel(this AbilityScore entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        return new AbilityScoreDataModel
        {
            AbilityScoreId = entity.Id,
            Name = entity.Name,
            Abbreviation = entity.Abbreviation,
            BaseScore = entity.BaseScore,
            AdditionalScore = entity.AdditionalScore,
            CalculatedScore = entity.CalculatedScore,
            CalculatedModifier = entity.CalculatedModifier
        };
    }

    public static List<AbilityScoreDataModel> AsAbilityScoreDataModels(this IEnumerable<AbilityScore> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        return [.. entities.Select(e => e.AsAbilityScoreDataModel())];
    }

    public static SkillDataModel AsSkillDataModel(this Skill entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        return new SkillDataModel
        {
            SkillId = entity.Id,
            Name = entity.Name,
            AbilityScoreId = entity.AbilityScoreId,
            AbilityScoreAbbreviation = entity.AbilityScoreAbbreviation,
            AbilityScoreModifier = entity.AbilityScoreModifier,
            AdditionalBonus = entity.AdditionalBonus,
            CalculatedBonus = entity.CalculatedBonus
        };
    }

    public static List<SkillDataModel> AsSkillDataModels(this IEnumerable<Skill> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        return [.. entities.Select(e => e.AsSkillDataModel())];
    }
}