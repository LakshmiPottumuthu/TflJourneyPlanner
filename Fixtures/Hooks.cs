
using Reqnroll;
using TflJourneyPlanner.Fixtures;

namespace TflJourneyPlanner.Fixtures;

[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext scenarioContext;

    public Hooks(ScenarioContext scenarioContext)
    {
        this.scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        var fixture = new BrowserFixture();
        await fixture.InitializeAsync();

        scenarioContext.Set(fixture, "BrowserFixture");
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (scenarioContext.TryGetValue(
            "BrowserFixture", out BrowserFixture? fixture)
            && fixture is not null)
        {
            await fixture.DisposeAsync();
        }
    }
}
