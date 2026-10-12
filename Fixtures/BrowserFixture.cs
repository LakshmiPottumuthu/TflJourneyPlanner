using Microsoft.Playwright;
using NUnit.Framework;
using TflJourneyPlanner.Config;

namespace TflJourneyPlanner.Fixtures;

public sealed class BrowserFixture : IAsyncDisposable
{
    private IPlaywright? playwright;
    private IBrowser? browser;
    private IBrowserContext? context;

    public IPage Page { get; private set; } = null!;

    public TestSettings Settings { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Settings = TestSettings.Load();

        string browserName = GetSetting("BROWSER", "chromium");
        bool headless = bool.Parse(
            GetSetting("HEADLESS", "false"));

       int timeout = Settings.TimeoutMilliseconds;

        playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = headless
        };

        browser = browserName.ToLowerInvariant() switch
        {
            "chromium" or "chrome" => await playwright.Chromium
                .LaunchAsync(launchOptions),

            "firefox" => await playwright.Firefox
                .LaunchAsync(launchOptions),

            "webkit" => await playwright.Webkit
                .LaunchAsync(launchOptions),

            _ => throw new ArgumentException(
                $"Unsupported browser '{browserName}'. " +
                "Use chromium, firefox, or webkit.")
        };

        context = await browser.NewContextAsync();

        Page = await context.NewPageAsync();

        Page.SetDefaultTimeout(timeout);
        Page.SetDefaultNavigationTimeout(timeout);
    }

private static string GetSetting(string name, string defaultValue)
{
    return Environment.GetEnvironmentVariable(name)
        ?? TestContext.Parameters.Get(name, defaultValue);
}

    public async ValueTask DisposeAsync()
    {
        if (context is not null)
            await context.CloseAsync();

        if (browser is not null)
            await browser.CloseAsync();

        playwright?.Dispose();
    }
}