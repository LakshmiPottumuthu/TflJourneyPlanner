public class JourneyTimeDetails
{
    public string DepartureTime { get; set; } = string.Empty;
    public string ArrivalTime { get; set; } = string.Empty;
    public string DisplayedDuration { get; set; } = string.Empty;
    public int CalculatedMinutes { get; set; }
    public int DisplayedMinutes { get; set; }
}