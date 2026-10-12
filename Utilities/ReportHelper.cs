
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace TflJourneyPlanner.Utilities
{
    public static class ReportHelper
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _currentTest;

        private static readonly string ReportDirectory =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..", "..", "..", "Reports"));

        private static readonly string ReportPath =
            Path.Combine(ReportDirectory, "TestReport.html");

        public static void Initialize()
        {
            Directory.CreateDirectory(ReportDirectory);

            Console.WriteLine($"Report path: {ReportPath}");

            var reporter = new ExtentSparkReporter(ReportPath);

            _extent = new ExtentReports();
            _extent.AttachReporter(reporter);
        }

        public static void CreateTest(string scenarioName)
        {
            _currentTest = _extent!.CreateTest(scenarioName);
        }

        public static void LogStep(
            string stepName,
            Status status,
            string? errorMessage = null)
        {
            if (_currentTest == null)
                return;

            switch (status)
            {
                case Status.Pass:
                    _currentTest.Pass(stepName);
                    break;

                case Status.Fail:
                    _currentTest.Fail(
                        $"{stepName}: {errorMessage}");
                    break;

                case Status.Skip:
                    _currentTest.Skip(stepName);
                    break;

                default:
                    _currentTest.Info(stepName);
                    break;
            }
        }

        public static void Pass(string message)
        {
            _currentTest?.Pass(message);
        }

        public static void Fail(Exception exception)
        {
            _currentTest?.Fail(exception);
        }

        public static void Flush()
        {
            _extent?.Flush();

            Console.WriteLine(
                $"Report generated: {File.Exists(ReportPath)}");

            Console.WriteLine($"Report location: {ReportPath}");
        }
    }
}
