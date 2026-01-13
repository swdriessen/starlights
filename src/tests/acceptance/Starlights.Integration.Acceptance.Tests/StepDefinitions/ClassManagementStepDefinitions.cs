using AwesomeAssertions;
using Starlights.Integration.Acceptance.Tests.Extensions;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.Classes;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.ClassFeatures;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Types.SubClasses;

namespace Starlights.Integration.Acceptance.Tests.StepDefinitions;

[Binding]
public class ClassManagementStepDefinitions
{
    private readonly IIntegrationHost _host;
    private readonly ScenarioContext _scenarioContext;
    private readonly ManageClassesDriver _driver;

    public ClassManagementStepDefinitions(IIntegrationHost host, ScenarioContext scenarioContext)
    {
        _host = host;
        _scenarioContext = scenarioContext;
        _driver = _host.GetDriver<ManageClassesDriver>();
    }

    [When(@"the content creator creates a ""([^""]*)"" class with a ""([^""]*)"" hit point die")]
    public async Task WhenTheContentCreatorCreatesAClassWithAHitPointDieAsync(string className, string hitDie)
    {
        var (size, amount) = ParseHitDie(hitDie);

        var properties = new ManageClassesDriver.CreateClassProperties
        {
            Name = className,
            HitPointDieSize = size,
            HitPointDieAmount = amount,
            Description = $"Description for {className}",
            ShortDescription = null
        };

        await _driver.CreateClassAsync(properties);
    }

    [Then(@"the ""([^""]*)"" class should have at least the following properties")]
    public async Task ThenTheClassShouldHaveAtLeastTheFollowingPropertiesAsync(string className, DataTable dataTable)
    {
        var actual = await _driver.GetClassByNameAsync(className);

        var expected = dataTable.Rows.Count == 0
            ? new ClassTableRow()
            : new ClassTableRow
            {
                Type = dataTable.Rows[0].TryGetValue("type", out var t) ? t : null,
                HitPointDie = dataTable.Rows[0].TryGetValue("hit point die", out var h) ? h : null
            };

        var assertions = new Dictionary<string, Action<ClassTableRow, ClassDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["type"] = (e, _) => e.Type.Should().Be("Class"),
            ["hit point die"] = (e, a) => a.HitPointDie.Should().BeEquivalentTo(e.HitPointDie)
        };

        dataTable.AssertProvidedProperties(expected, actual, assertions);
    }

    [Given(@"a class exists with the name ""([^""]*)""")]
    public async Task GivenAClassExistsWithTheNameAsync(string className)
    {
        var properties = new ManageClassesDriver.CreateClassProperties
        {
            Name = className,
            HitPointDieSize = 8,
            HitPointDieAmount = 1,
            Description = $"Description for {className}",
            ShortDescription = null
        };

        await _driver.CreateClassAsync(properties);
    }

    [When(@"the content creator creates a level (.*) class feature for the ""([^""]*)"" class with the name ""([^""]*)""")]
    public async Task WhenTheContentCreatorCreatesALevelClassFeatureForTheClassWithTheNameAsync(int level, string className, string featureName)
    {
        var parent = await _driver.GetClassByNameAsync(className);

        var properties = new ManageClassesDriver.CreateClassFeatureProperties
        {
            Name = featureName,
            Level = level,
            ParentClassId = parent.Id,
            ParentClassName = className,
            Description = $"Description for {featureName}"
        };

        await _driver.CreateClassFeatureAsync(properties);
    }

    [When(@"the content creator creates the following class features for the ""([^""]*)"" class:")]
    public async Task WhenTheContentCreatorCreatesTheFollowingClassFeaturesForTheClassAsync(string className, DataTable dataTable)
    {
        var parent = await _driver.GetClassByNameAsync(className);

        var rows = dataTable.CreateSet<ClassFeatureTableRow>();

        foreach (var row in rows)
        {
            var properties = new ManageClassesDriver.CreateClassFeatureProperties
            {
                Name = row.Name!,
                Level = row.FeatureLevel ?? 1,
                ParentClassId = parent.Id,
                ParentClassName = className,
                Description = $"Description for {row.Name}"
            };

            await _driver.CreateClassFeatureAsync(properties);
        }
    }

    [Then(@"the ""([^""]*)"" class feature should have at least the following properties")]
    public async Task ThenTheClassFeatureShouldHaveAtLeastTheFollowingPropertiesAsync(string featureName, DataTable dataTable)
    {
        var actual = await _driver.GetClassFeatureByNameAsync(featureName);

        var expected = dataTable.Rows.Count == 0
            ? new ClassFeatureTableRow()
            : new ClassFeatureTableRow
            {
                Name = dataTable.Rows[0].TryGetValue("name", out var n) ? n : null,
                Type = dataTable.Rows[0].TryGetValue("type", out var t) ? t : null,
                FeatureLevel = dataTable.Rows[0].TryGetValue("feature level", out var l) ? int.Parse(l) : null,
                FeatureParent = dataTable.Rows[0].TryGetValue("feature parent", out var p) ? p : null
            };

        var assertions = new Dictionary<string, Action<ClassFeatureTableRow, ClassFeatureDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["type"] = (e, _) => e.Type.Should().Be("Class Feature"),
            ["feature level"] = (e, a) => a.Level.Should().Be(e.FeatureLevel),
            ["feature parent"] = (e, a) => a.ParentName.Should().Be(e.FeatureParent)
        };

        dataTable.AssertProvidedProperties(expected, actual, assertions);
    }

    [When(@"the content creator creates a subclass for the ""([^""]*)"" class with the name ""([^""]*)""")]
    public async Task WhenTheContentCreatorCreatesASubclassForTheClassWithTheNameAsync(string className, string subClassName)
    {
        var parent = await _driver.GetClassByNameAsync(className);

        var properties = new ManageClassesDriver.CreateSubClassProperties
        {
            Name = subClassName,
            ParentClassId = parent.Id,
            ParentClassName = className,
            Description = $"Description for {subClassName}"
        };

        await _driver.CreateSubClassAsync(properties);
    }

    [Then(@"the ""([^""]*)"" subclass should have at least the following properties")]
    public async Task ThenTheSubclassShouldHaveAtLeastTheFollowingPropertiesAsync(string subClassName, DataTable dataTable)
    {
        var actual = await _driver.GetSubClassByNameAsync(subClassName);

        var expected = dataTable.Rows.Count == 0
            ? new SubClassTableRow()
            : new SubClassTableRow
            {
                Name = dataTable.Rows[0].TryGetValue("name", out var n) ? n : null,
                Type = dataTable.Rows[0].TryGetValue("type", out var t) ? t : null,
                FeatureParent = dataTable.Rows[0].TryGetValue("feature parent", out var p) ? p : null
            };

        var assertions = new Dictionary<string, Action<SubClassTableRow, SubClassDataModel>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (e, a) => a.Name.Should().Be(e.Name),
            ["type"] = (e, _) => e.Type.Should().Be("Sub Class"),
            ["feature parent"] = (e, a) => a.ParentName.Should().Be(e.FeatureParent)
        };

        dataTable.AssertProvidedProperties(expected, actual, assertions);
    }

    [Given(@"a class exists with the name ""([^""]*)"" with the following class features:")]
    public async Task GivenAClassExistsWithTheNameWithTheFollowingClassFeaturesAsync(string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator adds a new spellcasting rule to the ""([^""]*)"" class feature with the following details:")]
    public async Task WhenTheContentCreatorAddsANewSpellcastingRuleToTheClassFeatureWithTheFollowingDetailsAsync(string spellcasting, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ""([^""]*)"" element should have at least the following properties")]
    public async Task ThenTheElementShouldHaveAtLeastTheFollowingPropertiesAsync(string spellcasting, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"the following spells with their respective properties exists")]
    public async Task GivenTheFollowingSpellsWithTheirRespectivePropertiesExistsAsync(DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Given(@"a spellcaster class exists with the name ""([^""]*)"" and a spellcasting feature named ""([^""]*)""")]
    public async Task GivenASpellcasterClassExistsWithTheNameAndASpellcastingFeatureNamedAsync(string wizard, string spellcasting)
    {
        throw new PendingStepException();
    }

    [When(@"the content creator updates to the spellcasting rule of the ""([^""]*)"" class feature with the following ""([^""]*)"" spell list:")]
    public async Task WhenTheContentCreatorUpdatesToTheSpellcastingRuleOfTheClassFeatureWithTheFollowingSpellListAsync(string spellcasting, string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    [Then(@"the ""([^""]*)"" class feature should have at least the following spells in the ""([^""]*)"" spell list:")]
    public async Task ThenTheClassFeatureShouldHaveAtLeastTheFollowingSpellsInTheSpellListAsync(string spellcasting, string wizard, DataTable dataTable)
    {
        throw new PendingStepException();
    }

    private static (int size, int amount) ParseHitDie(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Hit die must be provided.", nameof(value));
        }

        var trimmed = value.Trim();
        if (!trimmed.StartsWith('D') && !trimmed.StartsWith('d'))
        {
            throw new FormatException($"Hit die '{value}' must be in form Dx or XdY.");
        }

        var sizePart = trimmed[1..];
        if (!int.TryParse(sizePart, out var size) || size <= 0)
        {
            throw new FormatException($"Hit die '{value}' has invalid size.");
        }

        return (size, 1);
    }

    private sealed class ClassTableRow
    {
        public string? Type { get; init; }
        public string? HitPointDie { get; init; }
    }

    private sealed class ClassFeatureTableRow
    {
        public string? Name { get; init; }
        public string? Type { get; init; }
        public int? FeatureLevel { get; init; }
        public string? FeatureParent { get; init; }
    }

    private sealed class SubClassTableRow
    {
        public string? Name { get; init; }
        public string? Type { get; init; }
        public string? FeatureParent { get; init; }
    }
}
