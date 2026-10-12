# TfL Journey Planner Automation

This project automates user journeys on the TfL journey planner using Reqnroll, NUnit, and Playwright.

## Prerequisites

- .NET SDK 10.0
- Node.js
- Git
- VS Code with the following extensions:
  - C# Dev Kit
  - Gherkin Step Navigator
## Project structure

- `Features/` — Gherkin feature files
- `StepDefinitions/` — Reqnroll step definitions
- `Pages/` — page object models and selectors
- `Fixtures/` — browser fixture and scenario hooks
- `Config/` — project configuration and environment settings
- `Models/` — test data models
- `Utilities/` — shared helpers and reporting helpers
- `Reports/` — generated test reports
- `test.runsettings` — parameterized browser, environment, and worker count
- `.github/workflows/tests.yml` — GitHub Actions workflow for CI
- `ParallelExecutionConfig.cs` — parallel execution configuration for the test suite

## Local setup

From the project root:

```powershell
dotnet restore
dotnet build
pwsh .\bin\Debug\net10.0\playwright.ps1 install --with-deps chromium
```

If the project has not yet been built, the Playwright browser install command will also create the required `bin` output folder.

## Run tests locally

Run the full suite with the configured runsettings:

dotnet test --settings test.runsettings

Run a specific category:

dotnet test --settings test.runsettings --filter "TestCategory=tagName"

Run with a browser override:

$env:BROWSER = "chromium"
$env:HEADLESS = "true"
dotnet test --settings test.runsettings

## Responsible use of AI

Copilot was used to draft parts of this README, suggest reporting improvements, and generate scenario code. The generated content was reviewed and adjusted to align with the project’s actual configuration and execution behavior.

**“Suggested Additional Test Scenarios”**

**Functional scenarios**
Same start and destination
Plan a journey for a future date and time 
Plan a journey using different transport modes 
Edit the Journey
Add the journey to favourites for future access
Request a step-free journey 
View journey details 
Handle a disrupted service
Swap the starting point and destination 
Use different location suggestions
Different modes of travelling
**Non-functional scenarios**
Responsive layout
Accessibility 
Performance - Load and stress testing
Browser compatibility
