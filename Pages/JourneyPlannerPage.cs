
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using TflJourneyPlanner.Config;
using TflJourneyPlanner.Utilities;

namespace TflJourneyPlanner.Pages;


public class ActiveTravelOption
{
    public string Duration { get; init; } = string.Empty;
    public string Distance { get; init; } = string.Empty;
}

public sealed class JourneyPlannerPage
{
    private readonly IPage page;
    private readonly TestSettings settings;

    public JourneyPlannerPage(
        IPage page,
        TestSettings settings)
    {
        this.page = page;
        this.settings = settings;
    }

    //Locators

    private ILocator StartingPoint =>
       page.GetByPlaceholder("From");

    private ILocator Destination =>
        page.Locator("#InputTo");

    private ILocator PlanJourneyButton =>
        page.GetByRole(
            AriaRole.Button,
            new() { Name = "Plan my journey", Exact = true });

    private ILocator UpdateJourneyButton =>
        page.GetByRole(
            AriaRole.Button,
            new() { Name = "Update journey", Exact = true });
    private ILocator EditPreferencesButton =>
        page.GetByRole(
            AriaRole.Button,
            new() { Name = "Edit preferences", Exact = true });
    private ILocator AcceptCookiesButton =>
        page.GetByRole(
            AriaRole.Button,
            new() { Name = "Accept all cookies", Exact = true });

    private ILocator WalkingSection =>
        page.Locator(".extra-journey-options h4");

    private ILocator EditJourney =>
       page.Locator("#fav-panel-save");

    private ILocator ViewDetailsButton =>
     page.GetByText("View details", new() { Exact = true })
    .Filter(new() { Visible = true });

    private ILocator RouteWithLeastWalking =>
       page.GetByText("Routes with least walking");

    private ILocator JourneyResults =>
       page.Locator("[data-testid='journey-results']");

    private ILocator GetOptionCard(string optionName) =>
        page.Locator(
            $"[data-testid='{optionName.ToLowerInvariant()}-result']");
    private ILocator JourneyCards =>
        page.Locator(".carousel-wrap");
    private const string DepartureTimeSelector =
      "//span[@class='time']/span[1]";

    private const string ArrivalTimeSelector =
        "//span[@class='time']/span[2]";

    private const string JourneyDurationSelector =
        "//div[@class='journey-time no-map']/span[1]";

    private ILocator GetDepartureTime(ILocator card) =>
        card.Locator(DepartureTimeSelector);

    private ILocator GetArrivalTime(ILocator card) =>
        card.Locator(ArrivalTimeSelector);

    private ILocator GetJourneyDuration(ILocator card) =>
        card.Locator(JourneyDurationSelector);

    private ILocator CompleteJourneyDetails =>
        page.Locator(".complete-journey-details");


    public async Task OpenAsync()
    {
        await page.GotoAsync(
            settings.BaseUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        await WaitHelper.WaitForVisibleAsync(AcceptCookiesButton);
        await AcceptCookiesButton.ClickAsync();

        await WaitHelper.reloadPageAndWaitForVisibleAsync(page, StartingPoint);


    }

    public async Task<string> GetPageTitleAsync()
    {
        return await page.TitleAsync();
    }

    public Task SelectStartingPointAsync(string stationName)
    {
        return SelectStationFromAutocompleteAsync(
            StartingPoint, stationName);
    }

    public Task SelectDestinationAsync(string stationName)
    {
        return SelectStationFromAutocompleteAsync(
            Destination, stationName);
    }

    private async Task SelectStationFromAutocompleteAsync(
     ILocator input,
     string stationName)
    {
        // Type only part of the station name to trigger autocomplete.
        string searchText = stationName.Split(' ')[0];
        await WaitHelper.FillAsync(input, searchText);

        var suggestions = page.Locator(
            "[role='option'], " +
            ".ui-menu-item, " +
            ".autocomplete-suggestion, " +
            ".tt-suggestion, " +
            ".pac-item");

        // Find the suggestion matching the feature-file value.
        var matchingSuggestion = suggestions.Filter(
            new LocatorFilterOptions
            {
                HasTextRegex = new Regex(
                    $@"^\s*{Regex.Escape(stationName.Trim())}\s*$",
                    RegexOptions.IgnoreCase)
            }).First;

        await WaitHelper.WaitForVisibleAsync(matchingSuggestion);

        // Click only the matching autocomplete suggestion.
        await matchingSuggestion.ClickAsync();

    }

    public async Task PlanJourneyAsync()
    {
        await WaitHelper.WaitForVisibleAsync(PlanJourneyButton);
        await PlanJourneyButton.ClickAsync();

        // Wait for the journey results page to load.
        await page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded);
    }

    public async Task EditJourneyPreferencesAsync()
    {
        await WaitHelper.WaitForVisibleAsync(EditPreferencesButton);
        await EditPreferencesButton.ClickAsync();

        // Wait for the journey results page to load.
        await page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded);
    }

    public async Task<bool> AreJourneyResultsDisplayedAsync()
    {
        await WaitHelper.WaitForVisibleAsync(EditJourney);
        return await JourneyResults.IsVisibleAsync();
    }

    public async Task<bool> AreWalkingAndCyclingSectionsAvailableAsync()
    {
        await WaitHelper.StaticWaitForVisibleAsync(30000);
        var walkingSection = WalkingSection
            .Filter(new LocatorFilterOptions
            {
                HasTextRegex = new Regex(@"^\s*Walking\s*$",
                    RegexOptions.IgnoreCase)
            });

        var cyclingSection = WalkingSection
            .Filter(new LocatorFilterOptions
            {
                HasTextRegex = new Regex(@"^\s*Cycling\s*$",
                    RegexOptions.IgnoreCase)
            });

        return await walkingSection.IsVisibleAsync()
            && await cyclingSection.IsVisibleAsync();
    }

    public async Task SelectRoutesWithLeastWalkingAsync()
    {
        await RouteWithLeastWalking.ClickAsync();
    }

    public async Task UpdateJourneyAsync()
    {
        await UpdateJourneyButton.WaitForAsync(
        new() { State = WaitForSelectorState.Attached });

        await UpdateJourneyButton.ScrollIntoViewIfNeededAsync();
        await UpdateJourneyButton.ClickAsync();

        // Wait for the journey results page to load.
        await page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded);
    }

    public async Task<bool> AreLocationFieldsEmptyAsync()
    {
        string from = await StartingPoint.InputValueAsync();
        string to = await Destination.InputValueAsync();

        return string.IsNullOrWhiteSpace(from)
            && string.IsNullOrWhiteSpace(to);
    }
    public async Task ClearLocationsAsync()
    {
        await Destination.ClearAsync();
        await StartingPoint.ClearAsync();
    }

    public async Task EnterStartingPointAsync(string location)
    {
        await WaitHelper.EnterLocationAsync(StartingPoint, location);
    }

    public async Task EnterDestinationAsync(string location)
    {
        await WaitHelper.EnterLocationAsync(Destination, location);
    }

    public async Task ValidateActiveTravelOptionAsync(string optionName)
    {
        var card = GetOptionCard(optionName);

        await Assertions.Expect(card).ToBeVisibleAsync();

        var durationLocator = card.Locator(".duration");
        var distanceLocator = card.Locator(".distance");

        await Assertions.Expect(durationLocator).ToBeVisibleAsync();
        await Assertions.Expect(distanceLocator).ToBeVisibleAsync();

        var duration = (await durationLocator.InnerTextAsync()).Trim();
        var distance = (await distanceLocator.InnerTextAsync()).Trim();

        Assert.True(
            WaitHelper.IsValidDuration(duration),
            $"{optionName}: invalid duration '{duration}'.");

        Assert.True(
            WaitHelper.IsValidDistance(distance),
            $"{optionName}: invalid distance '{distance}'.");

        Console.WriteLine(
            $"{optionName}: duration = {duration}, distance = {distance}");
    }

    public async Task<List<JourneyTimeDetails>>
     GetUpdatedJourneyTimesAsync()
    {
        var journeys = new List<JourneyTimeDetails>();
        int count = await JourneyCards.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var card = JourneyCards.Nth(i);

            string departureText =
                (await GetDepartureTime(card).InnerTextAsync()).Trim();

            string arrivalText =
                (await GetArrivalTime(card).InnerTextAsync()).Trim();

            string durationText =
                (await GetJourneyDuration(card).InnerTextAsync()).Trim();

            if (!DateTime.TryParseExact(
                    departureText, "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var departure))
            {
                throw new FormatException(
                    $"Invalid departure time: {departureText}");
            }

            if (!DateTime.TryParseExact(
                    arrivalText, "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var arrival))
            {
                throw new FormatException(
                    $"Invalid arrival time: {arrivalText}");
            }

            var elapsed = arrival - departure;

            if (elapsed.TotalMinutes < 0)
                elapsed = elapsed.Add(TimeSpan.FromDays(1));

            int displayedMinutes =
                await WaitHelper.ParseDuration(durationText);

            journeys.Add(new JourneyTimeDetails
            {
                DepartureTime = departureText,
                ArrivalTime = arrivalText,
                DisplayedDuration = durationText,
                CalculatedMinutes = (int)elapsed.TotalMinutes,
                DisplayedMinutes = displayedMinutes
            });
        }

        return journeys;
    }

    public async Task ViewDetailsOfFirstJourneyOptionAsync()
    {
        await ViewDetailsButton.ClickAsync();
    }

    public async Task<string> GetExpandedJourneyDetailsAsync()
    {
        var card = JourneyCards.First;
        await card.GetByRole(
            AriaRole.Button,
            new() { Name = "Hide details" })
            .WaitForAsync();

        return await card.InnerTextAsync();
    
    }
}
