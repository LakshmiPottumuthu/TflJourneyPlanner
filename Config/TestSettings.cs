
using System.Text.Json;

namespace TflJourneyPlanner.Config;

public sealed class TestSettings
{
    public string Environment { get; set; } = "QA";
    public string BaseUrl { get; set; } = "https://tfl.gov.uk";
    public string JourneyPlannerPath { get; set; } = "/plan-a-journey";
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; } = false;
    public int TimeoutMilliseconds { get; set; } = 15000;

    public static TestSettings Load()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Config",
            "appsettings.json");

        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Configuration file not found: {path}");

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<TestSettings>(json)
            ?? throw new InvalidOperationException(
                "Unable to load test configuration.");
    }

    public string JourneyPlannerUrl =>
        $"{BaseUrl.TrimEnd('/')}/{JourneyPlannerPath.TrimStart('/')}";
}
