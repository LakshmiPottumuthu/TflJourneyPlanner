
using NUnit.Framework;
using Reqnroll;
using TflJourneyPlanner.Fixtures;
using TflJourneyPlanner.Pages;
using TflJourneyPlanner.Utilities;

namespace TflJourneyPlanner.StepDefinitions;

[Binding]
public sealed class JourneyPlannerSteps
{
    private readonly BrowserFixture fixture;
    private readonly JourneyPlannerPage journeyPlannerPage;

    public JourneyPlannerSteps(ScenarioContext scenarioContext)
    {
        fixture = scenarioContext.Get<BrowserFixture>(
            "BrowserFixture");

        journeyPlannerPage = new JourneyPlannerPage(
            fixture.Page,
            fixture.Settings);
    }

    [Given(@"I open the TfL Journey Planner")]
    public async Task GivenIOpenTheTfLJourneyPlanner()
    {
        await journeyPlannerPage.OpenAsync();
    }

    [When("I select {string} as the starting point")]
    public async Task WhenISelectStartingPoint(string stationName)
    {
        await journeyPlannerPage.SelectStartingPointAsync(stationName);
    }

    [When("I select {string} as the destination")]
    public async Task WhenISelectDestination(string stationName)
    {
        await journeyPlannerPage.SelectDestinationAsync(stationName);
    }

    [When("I plan my journey")]
    public async Task WhenIPlanMyJourney()
    {
        await journeyPlannerPage.PlanJourneyAsync();
    }

    [Then("the journey results should be displayed")]
    public async Task ThenJourneyResultsShouldBeDisplayed()
    {
        Assert.That(
            await journeyPlannerPage.AreJourneyResultsDisplayedAsync(),
            Is.True,
            "Journey results were not displayed.");
    }

    [Then("walking and cycling journey times should be available")]
    public async Task ThenWalkingAndCyclingJourneyTimesShouldBeAvailable()
    {
        bool sectionsAvailable =
          await journeyPlannerPage.AreWalkingAndCyclingSectionsAvailableAsync();

        Assert.That(
         sectionsAvailable,
         Is.True,
         "Walking and/or Cycling sections are not visible on the journey results page.");
    }
    [When("I edit journey preferences")]
    public async Task IEditJourneyPreferences()
    {
        await journeyPlannerPage.EditJourneyPreferencesAsync();
    }
    [When("I select routes with least walking")]
    public async Task ISelectRoutesWithLeastWalking()
    {
        await journeyPlannerPage.SelectRoutesWithLeastWalkingAsync();
    }
    [When("I update the journey")]
    public async Task IUpdateTheJourney()
    {
        await journeyPlannerPage.UpdateJourneyAsync();
    }

    [When(@"I enter ""(.*)"" as the starting point")]
    public async Task WhenIEnterStartingPoint(string location)
    {
        await journeyPlannerPage.EnterStartingPointAsync(location);
    }

    [When(@"I enter ""(.*)"" as the destination")]
    public async Task WhenIEnterDestination(string location)
    {
        await journeyPlannerPage.EnterDestinationAsync(location);
    }
    [Then(@"no journey results should be displayed")]
    public async Task ThenNoJourneyResultsShouldBeDisplayed()
    {
        Assert.That(
            await journeyPlannerPage.AreJourneyResultsDisplayedAsync(),
            Is.False,
            "Journey results were displayed for invalid locations.");
    }

    [When(@"I leave the starting point and destination empty")]
    public async Task WhenILeaveLocationsEmpty()
    {
        await journeyPlannerPage.ClearLocationsAsync();


    }

    [Then(@"the journey should not be planned")]
    public async Task ThenJourneyShouldNotBePlanned()
    {
        Assert.That(
            await journeyPlannerPage.AreJourneyResultsDisplayedAsync(),
            Is.False,
            "A journey was planned even though no locations were entered.");
    }


    [Then(@"the walking option should display a valid duration and distance")]
    public async Task ThenWalkingOptionShouldDisplayValidDurationAndDistance()
    {
        await journeyPlannerPage.ValidateActiveTravelOptionAsync("Walking");


    }

    [Then(@"the cycling option should display a valid duration and distance")]
    public async Task ThenCyclingOptionShouldDisplayValidDurationAndDistance()
    {
        await journeyPlannerPage.ValidateActiveTravelOptionAsync("Cycling");
    }

    [Then(@"the updated journey time should be valid")]
    public async Task ThenTheUpdatedJourneyTimeShouldBeValid()
    {
        await journeyPlannerPage.GetUpdatedJourneyTimesAsync();
    }
    [When("I view details of the first journey option")]
    public async Task IViewDetailsOfTheFirstJourneyOption()
    {
        await journeyPlannerPage.ViewDetailsOfFirstJourneyOptionAsync();
    }
    [Then("the complete access information should be displayed")]
    public async Task ThenTheCompleteAccessInformationShouldBeDisplayed()
    {
        var details = await journeyPlannerPage
              .GetExpandedJourneyDetailsAsync();

        Assert.False(string.IsNullOrWhiteSpace(details));
        Assert.That(details, Does.Contain("Hide details"));
        Assert.That(details, Does.Contain("View directions"));
    }


}
