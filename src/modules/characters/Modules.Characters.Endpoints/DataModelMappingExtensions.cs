using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Skills;
using Starlights.Modules.Characters.Domain.Classes; // added
using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows;
using Starlights.Modules.Characters.Endpoints.Characters.Skills;
using Starlights.Modules.Characters.Endpoints.Models;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses; // added

namespace Starlights.Modules.Characters.Endpoints;

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

    public static SavingThrowDataModel AsSavingThrowDataModel(this SavingThrow entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        return new SavingThrowDataModel
        {
            SavingThrowId = entity.Id,
            Name = entity.Name,
            AbilityScoreId = entity.AbilityScoreId,
            AbilityScoreAbbreviation = entity.AbilityScoreAbbreviation,
            AbilityScoreModifier = entity.AbilityScoreModifier,
            AdditionalBonus = entity.AdditionalBonus,
            CalculatedBonus = entity.CalculatedBonus
        };
    }

    public static List<SavingThrowDataModel> AsSavingThrowDataModels(this IEnumerable<SavingThrow> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        return [.. entities.Select(e => e.AsSavingThrowDataModel())];
    }

    // character classes
    public static CharacterClassDataModel AsCharacterClassDataModel(this CharacterClass entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        return new CharacterClassDataModel
        {
            CharacterClassId = entity.Id,
            RegistrationId = entity.Registration,
            Name = entity.Name,
            Level = entity.Level,
            IsPrimary = entity.IsPrimary
        };
    }

    public static List<CharacterClassDataModel> AsCharacterClassDataModels(this IEnumerable<CharacterClass> entities)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        return [.. entities.Select(e => e.AsCharacterClassDataModel())];
    }
}
