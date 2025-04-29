using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Syncfusion.UITestHelpers.Core;

namespace Syncfusion.UITestHelpers.NUnit
{
    public abstract class UITestBase : UITestContextBase
    {
        public UITestBase(TestDevice testDevice) : base(testDevice)
        {
        }
        IConfig config;
        public static string name;
        public string Classname;
        [SetUp]
        public void RecordTestSetup()
        {
            name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
            Classname = TestContext.CurrentContext.Test.ClassName ?? TestContext.CurrentContext.Test.Name;
            TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Start");
            UITestExtendReport.testStartTime = DateTime.Now; // Start time for each test
        }

        public void RecordTestTeardown()
        {
            name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
            Classname = TestContext.CurrentContext.Test.ClassName ?? TestContext.CurrentContext.Test.Name;
            TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Stop");
        }

        protected virtual void FixtureSetup()
        {
            name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
            Classname = TestContext.CurrentContext.Test.ClassName ?? TestContext.CurrentContext.Test.Name;
            TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureSetup)} for {name}");
        }

        protected virtual void FixtureTeardown()
        {
            name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
            Classname = TestContext.CurrentContext.Test.ClassName ?? TestContext.CurrentContext.Test.Name;

            TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureTeardown)} for {name}");
        }
        [TearDown]
        public void UITestBaseTearDown()
        {
            RecordTestTeardown();
            Reset();

            if (App.AppState == ApplicationState.NotRunning)
            {
                Assert.Fail("The app was expected to be running still, investigate as possible crash");
            }

            // Get test status
            string status = TestContext.CurrentContext.Result.Outcome.Status.ToString();

            // Update counts before generating report
            UITestExtendReport.totalTests++;

            if (status == "Passed")
            {
                UITestExtendReport.passedTests++;
            }
            else if (status == "Failed")
            {
                UITestExtendReport.failedTests++;
            }
            else if (status == "Error")
            {
                UITestExtendReport.errorTests++;
            }

            // Capture execution time
            UITestExtendReport.testEndTime = DateTime.Now;
            TimeSpan executionTime = UITestExtendReport.testEndTime - UITestExtendReport.testStartTime;
            
            // Determine test status
             status = TestContext.CurrentContext.Result.Outcome.Status.ToString();


            string testName = TestContext.CurrentContext.Test.Name;
            UITestExtendReport.executionTime = executionTime.TotalSeconds;


            UITestExtendReport.testDurations.Add(new Tuple<string, double>(testName, UITestExtendReport.executionTime));

            UITestExtendReport.totalExecutionTime = UITestExtendReport.testDurations.Sum(td => td.Item2);
            UITestExtendReport.UpdateTestData(name , UITestExtendReport.executionTime);

            UITestExtendReport.UpdateTestResult(name, status, UITestExtendReport.testException);

        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            UITestExtendReport.totalTests = 0;
            UITestExtendReport.passedTests = 0;
            UITestExtendReport.failedTests = 0;
            UITestExtendReport.errorTests = 0;
            UITestExtendReport.InitializeReport();
            InitialSetup(UITestContextSetupFixture.ServerContext);
            FixtureSetup();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            FixtureTeardown();
            //UITestExtendReport.totalExecutionTime = UITestExtendReport.testDurations.Sum(td => td.Item2);

            UITestExtendReport.jsforreport();
        }


        void SaveDeviceDiagnosticInfo([CallerMemberName] string? note = null)
        {
            var types = App.GetLogTypes().ToArray();
            TestContext.Progress.WriteLine($">>>>> {DateTime.Now} Log types: {string.Join(", ", types)}");

            foreach (var logType in new[] { "logcat" })
            {
                if (!types.Contains(logType, StringComparer.InvariantCultureIgnoreCase))
                    continue;

                var logsPath = GetGeneratedFilePath($"AppLogs-{logType}.log", note);
                if (logsPath is not null)
                {
                    var entries = App.GetLogEntries(logType);
                    File.WriteAllLines(logsPath, entries);

                    AddTestAttachment(logsPath, Path.GetFileName(logsPath));
                }
            }
        }

        protected void SaveUIDiagnosticInfo([CallerMemberName] string? note = null)
        {
            var screenshotPath = GetGeneratedFilePath("ScreenShot.png", note);
            if (screenshotPath is not null)
            {
                _ = App.Screenshot(screenshotPath);

                AddTestAttachment(screenshotPath, Path.GetFileName(screenshotPath));
            }

            var pageSourcePath = GetGeneratedFilePath("PageSource.txt", note);
            if (pageSourcePath is not null)
            {
                File.WriteAllText(pageSourcePath, App.ElementTree);

                AddTestAttachment(pageSourcePath, Path.GetFileName(pageSourcePath));
            }
        }

        string? GetGeneratedFilePath(string filename, string? note = null)
        {
            // App could be null if UITestContext was not able to connect to the test process (e.g. port already in use etc...)
            if (UITestContext is null)
                return null;

            if (string.IsNullOrEmpty(note))
                note = "-";
            else
                note = $"-{note}-";

            filename = $"{Path.GetFileNameWithoutExtension(filename)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(filename)}";

            var logDir =
                Path.GetDirectoryName(Environment.GetEnvironmentVariable("APPIUM_LOG_FILE") ??
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))!;

            var name =
                TestContext.CurrentContext.Test.MethodName ??
                TestContext.CurrentContext.Test.Name;

            return Path.Combine(logDir, $"{name}-{_testDevice}{note}{filename}");
        }

        void AddTestAttachment(string filePath, string? description = null)
        {
            try
            {
                TestContext.AddTestAttachment(filePath, description);
            }
            catch (FileNotFoundException e) when (e.Message == "Test attachment file path could not be found.")
            {
                // Add the file path to better troubleshoot when these errors occur
                throw new FileNotFoundException($"Test attachment file path could not be found: '{filePath}' {description}", e);
            }
        }
    }
}