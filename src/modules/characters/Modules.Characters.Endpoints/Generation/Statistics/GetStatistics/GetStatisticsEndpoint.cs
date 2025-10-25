using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Services.Statistics;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Generation.Statistics.GetStatistics;

public sealed class GetStatisticsEndpoint : Endpoint<GetStatisticsRequest, GetStatisticsResponse>
{
    private readonly ILogger<GetStatisticsEndpoint> _logger;
    private readonly IPersistence _persistence;
    private readonly StatisticsCalculator _statisticsCalculator;

    public GetStatisticsEndpoint(
        ILogger<GetStatisticsEndpoint> logger,
        IPersistence persistence,
        StatisticsCalculator statisticsCalculator)
    {
        _logger = logger;
        _persistence = persistence;
        _statisticsCalculator = statisticsCalculator;
    }

    public override void Configure()
    {
        Get("{characterId:guid}/statistics");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetStatisticsRequest req, CancellationToken ct)
    {
        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(req.CharacterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        var characterRegistrations = await registrationRepository.GetRegistrationsAsync(new(req.CharacterId));

        var result = _statisticsCalculator.Calculate(character, characterRegistrations);

        await _persistence.SaveChangesAsync(); // temporary: save any changes made during calculation TODO: remove when calculation is done after processing too


        var models = result.Statistics
            .Where(g => g.GetStatisticValues().Count > 0 && !g.GroupName.Contains(":half"))
            .Select(group => new StatisticGroupDataModel
            {
                GroupName = group.GroupName,
                TotalValue = group.Sum(),
                IsFinalized = group.IsCompleted,
                Values = [.. group.GetStatisticValues().Select(v => new StatisticValueDataModel {
                    Source = v.Source,
                    Value = v.Value,
                    DisplayName = v.DisplayName,
                    RuleId = v.RuleId })]
            }).ToList();

        await Send.OkAsync(new GetStatisticsResponse { Statistics = models }, ct);
    }
}
