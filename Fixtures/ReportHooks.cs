
using Reqnroll;
using AventStack.ExtentReports;
using TflJourneyPlanner.Utilities;

namespace TflJourneyPlanner.Fixtures
{
    [Binding]
    public class ReportHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public ReportHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            ReportHelper.Initialize();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ReportHelper.CreateTest(
                _scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void AfterStep()
        {
            var stepInfo =
                _scenarioContext.StepContext.StepInfo;

            string stepName =
                $"{stepInfo.StepDefinitionType} {stepInfo.Text}";

            if (_scenarioContext.TestError != null)
            {
                ReportHelper.LogStep(
                    stepName,
                    Status.Fail,
                    _scenarioContext.TestError.Message);
            }
            else
            {
                ReportHelper.LogStep(
                    stepName,
                    Status.Pass);
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.TestError != null)
            {
                ReportHelper.Fail(
                    _scenarioContext.TestError);
            }
            else
            {
                ReportHelper.Pass("Scenario passed");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            ReportHelper.Flush();
        }
    }
}
