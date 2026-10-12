
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace TflJourneyPlanner.Utilities;

public static class WaitHelper
{
    public static Task StaticWaitForVisibleAsync(
        int timeoutMilliseconds = 30000)
    {
        return Task.Delay(timeoutMilliseconds);
      
    }

    public static Task WaitForVisibleAsync(
        ILocator locator,
        int timeoutMilliseconds = 15000)
    {
        return locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMilliseconds
        });
    }

    public static async Task FillAsync(
        ILocator locator,
        string value,
        int timeoutMilliseconds = 15000)
    {
        await WaitForVisibleAsync(locator, timeoutMilliseconds);
        await locator.FillAsync(value);
    }

    public static async Task reloadPageAndWaitForVisibleAsync(
        IPage page,
        ILocator locator,
        int timeoutMilliseconds = 15000)
    {
        await page.ReloadAsync();
        await WaitForVisibleAsync(locator, timeoutMilliseconds);
    }

      public static async Task reloadPage(
        IPage page)
            {
        await page.ReloadAsync();
    }

    public static async Task EnterLocationAsync(
    ILocator input,
    string location)
{
    await WaitHelper.WaitForVisibleAsync(input);
    await input.FillAsync(location);
}

 public static bool IsValidDuration(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var match = Regex.Match(
            value.Trim(),
            @"^(?:(\d+)\s*hours?\s*)?" +
            @"(?:(\d+)\s*mins?)?$",
            RegexOptions.IgnoreCase);

        if (!match.Success ||
            (!match.Groups[1].Success &&
             !match.Groups[2].Success))
        {
            return false;
        }

        int hours = match.Groups[1].Success
            ? int.Parse(match.Groups[1].Value,
                CultureInfo.InvariantCulture)
            : 0;

        int minutes = match.Groups[2].Success
            ? int.Parse(match.Groups[2].Value,
                CultureInfo.InvariantCulture)
            : 0;

        return hours * 60 + minutes > 0;
    }

    public static bool IsValidDistance(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var match = Regex.Match(
            value.Trim(),
            @"^(\d+(?:\.\d+)?)\s*(km|m|mi|miles?)$",
            RegexOptions.IgnoreCase);

        return match.Success &&
            double.TryParse(
                match.Groups[1].Value,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out double distance) &&
            distance > 0;
    }

     public static async Task<int> ParseDuration(string text)
    {
        var match = Regex.Match(
            text,
            @"^(?:(\d+)\s*hours?\s*)?(?:(\d+)\s*mins?)?$",
            RegexOptions.IgnoreCase);

        Assert.True(match.Success &&
            (match.Groups[1].Success || match.Groups[2].Success),
            $"Invalid journey duration: '{text}'");

        int hours = match.Groups[1].Success
            ? int.Parse(match.Groups[1].Value,
                CultureInfo.InvariantCulture)
            : 0;

        int minutes = match.Groups[2].Success
            ? int.Parse(match.Groups[2].Value,
                CultureInfo.InvariantCulture)
            : 0;

        int totalMinutes = (hours * 60) + minutes;

        Assert.True(totalMinutes > 0,
            $"Journey duration must be positive: '{text}'");

        return totalMinutes;
    }
    
}
