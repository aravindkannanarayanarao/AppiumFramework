
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Text;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;


namespace Syncfusion.UITestHelpers.NUnit
{
    public static class UITestExtendReport
    {
        private static List<Tuple<string, double>> testDurations1 = new List<Tuple<string, double>>(); // Stores (TestName, Duration)
        public static List<Tuple<string, double>> testDurations = new List<Tuple<string, double>>(); // Stores (TestName, Duration)
        public static List<Tuple<string, double>> testStatuses = new List<Tuple<string, double>>(); // Stores test name + status
        public static List<Tuple<string, double>> testExecutionTimes = new List<Tuple<string, double>>(); // Test name + execution time
        //public static List<Tuple<string, string>> testException = new List<Tuple<string, string>>(); // Test name + execution time
        public static string testException;
        public static string testDetailsTable;
        public static double executionTime;
        public static double totalExecutionTime;
        public static DateTime testStartTime;
        public static DateTime testEndTime;
        public static DateTime testExecutionTime;
        public static string report;
        public static string htmlhomepage;
        public static string htmldashbard;
        public static string updateStats;
        public static int totalTests;
        public static int passedTests;
        public static int failedTests;
        public static int errorTests;
        // Create test details table dynamically

        public static void InitializeReport()
        {
        }
        public static List<(string testName, string exception)> testCases = new List<(string, string)>();

        public static void UpdateTestResult(string name, string status, string testException)
        {
            // Ensure unique test cases and update with specific exception
            var existingIndex = testCases.FindIndex(tc => tc.testName == name);
            if (existingIndex != -1)
            {
                testCases[existingIndex] = (name, testException); // Update existing entry
            }
            else
            {
                testCases.Add((name, testException)); // Add new entry
            }

            // Ensure valid HTML-safe ID
            string sanitizedTestName = name.Replace(" ", "_").Replace("-", "_");

            // Build HTML content
            StringBuilder htmlcontent = new StringBuilder();
            htmlcontent.AppendLine(@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Home - Test Cases</title>
    <link href='https://fonts.googleapis.com/icon?family=Material+Icons' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css'>
    <style>
        body {
            font-family: 'Roboto', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            color: #333;
            display: flex;
            flex-direction: column;
            height: 100vh;
        }
        nav {
            background-color: #6200ea;
            color: white;
            padding: 16px;
            position: sticky;
            top: 0;
            z-index: 1000;
        }
        nav a {
            color: white;
            text-decoration: none;
            font-weight: bold;
            margin: 0 16px;
            transition: 0.3s;
            padding: 8px 16px;
            border-radius: 4px;
        }
        nav a:hover {
            background-color: rgba(255, 255, 255, 0.1);
        }
        .container {
            display: flex;
            flex: 1;
            padding: 20px;
            gap: 30px;
        }
        .test-cases {
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            width: 25%;
            overflow-y: auto;
        }
        .test-cases ul {
            list-style-type: none;
            padding: 0;
        }
        .test-cases li {
            padding: 8px 0;
            border-bottom: 1px solid #eee;
            margin-bottom: 8px;
            cursor: pointer;
        }
        .test-cases li:hover {
            background-color: #f1f1f1;
        }
.test-cases {
    background-color: #ffffff;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    width: 25%;
    height: 80vh; /* Fixed height */
    overflow-y: auto; /* Scrollable */
}

/* Test Case Details (Should NOT Scroll) */
.test-case-table {
    background-color: #ffffff;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    flex: 1; 
    height: auto; /* Ensures it doesn't scroll */
    overflow-y: hidden; /* Prevents scrolling */
    position: sticky;
    top: 20px; /* Keeps it visible when scrolling */
    display: none;
}
.nav-logo {
    height: 40px; /* Adjust size as needed */
    margin-right: 16px;
    vertical-align: middle;
}
nav a img {
    display: inline-block;
}

        .test-case-table {
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            flex: 1;
            display: none;
            overflow-y: auto;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        th, td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }
        th {
            background-color: #6200ea;
            color: white;
        }

		.test-cases {
    background-color: #ffffff;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    width: 25%;
    height: 80vh; /* Fixed height to enable scrolling */
    overflow-y: auto; /* Enable vertical scrolling */
}
        .stats {
            display: flex;
            justify-content: space-between;
            background-color: #ffffff;
            padding: 16px;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            margin-top: 20px;
            position: sticky;
            bottom: 0;
            width: 98%;
        }
        .stats div {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .stats div i {
            font-size: 24px;
            color: #6200ea;
        }
    </style>
</head>
<body>

<nav> 
    <a href=""home.html"">
        <img src=""../Images/logo.png"" alt=""Logo"" class=""nav-logo"">
    </a>
    <a href='home.html'>Home</a>
    <a href='dashboard.html'>Dashboard</a>
</nav>

<div class='container'>
    <div class='test-cases'>
        <h3>Test Cases List</h3>
        <ul id='testcaseslist'>");

            // Generate Test Cases List
            foreach (var (testCase, _) in testCases)
            {
                string sanitizedTest = testCase.Replace(" ", "_").Replace("-", "_");
                htmlcontent.AppendLine($"<li onclick=\"showTestCaseDetails('{sanitizedTest}')\">{testCase}: {status}</li>");
            }

            htmlcontent.AppendLine(@"</ul>
    </div>");

            // Generate Test Case Result Sections
            foreach (var (testCase, exception) in testCases)
            {
                string sanitizedTest = testCase.Replace(" ", "_").Replace("-", "_");
                htmlcontent.AppendLine($@"
    <div id='{sanitizedTest}' class='test-case-table'>
        <h3>{testCase} Results</h3>
        <table>
            <thead>
                <tr>
                    <th>Expected Image</th>
                    <th>Output Image</th>
                    <th>Differences Image</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><img src='../Images/snapshots/{testCase}.png' width='300'></td>
                    <td><img src='../Images/snapshots-output/{testCase}.png' width='300'></td>
                    <td><img src='../Images/snapshots-diff/{testCase}-diff.png' width='300'></td>
                </tr>
            </tbody>
        </table>
<div><i>{exception}</i></div>
    </div>");
            }

            htmlcontent.AppendLine(@"
</div>

<div class='stats'>
    <div><i class='fas fa-check-circle'></i> Total Ran: <span id='totalTests' data-value='{totalTests}'>0</span></div>
    <div><i class='fas fa-times-circle'></i> Total Failed: <span id='failedTests' data-value='{failedTests}'>0</span></div>
    <div><i class='fas fa-exclamation-circle'></i> Total Errors: <span id='skippedTests' data-value='{skippedTests}'>0</span></div>
    <div><i class='fas fa-check-circle'></i> Total Passed: <span id='passedTests' data-value='{passedTests}'>0</span></div>
</div>
<script src='updateStats.js'></script>

</body>
</html>");

            // Ensure directory exists before writing the file
            if (!Directory.Exists(report))
            {
                Directory.CreateDirectory(report);
            }
            File.WriteAllText(htmlhomepage, htmlcontent.ToString());
        }


        public static void UpdateTestData(string name, double executionTime)
        {
            
            // Ensure test case exists in both lists
            if (!testStatuses.Any(x => x.Item1 == name))
            {
                testExecutionTimes.Add(new Tuple<string, double>(name, executionTime)); // Add execution time
            }

            // Generate HTML Report
            StringBuilder htmlcontent1 = new StringBuilder();
            htmlcontent1.AppendLine(@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Test Report</title>
    <style>
        body { font-family: 'Arial', sans-serif; background-color: #f5f5f5; color: #333; padding: 20px; }

        nav {
            background-color: #6200ea;
            color: white;
            padding: 16px;
            position: sticky;
            top: 0;
            z-index: 1000;
        }
        nav a {
            color: white;
            text-decoration: none;
            font-weight: bold;
            margin: 0 16px;
            transition: 0.3s;
            padding: 8px 16px;
            border-radius: 4px;
        }
        nav a:hover {
            background-color: rgba(255, 255, 255, 0.1);
        }
        table { width: 100%; border-collapse: collapse; margin: 20px 0; background: white; }
        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
        th { background: #6200ea; color: white; }
        .container { display: flex; flex-wrap: wrap; gap: 20px; }
        .summary, .testcase-detail-table { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1); width: 45%; }
        h3 { margin-bottom: 10px; }
    </style>
</head>
<body>
<nav> 
    <a href=""home.html"">
        <img src=""../Images/logo.png"" alt=""Logo"" class=""nav-logo"">
    </a>
    <a href='home.html'>Home</a>
    <a href='dashboard.html'>Dashboard</a>
</nav>

<h2>Automated Test Execution Report</h2>

<div class='container'>

    <!-- Summary Table -->
    <div class='summary'>
        <h3>Status Summary</h3>
        <table class='table-list'>
            <thead>
                <tr><th>Status</th><th>Count</th></tr>
            </thead>
            <tbody>
                <tr><td>Passed</td><td>" + passedTests + @"</td></tr>
                <tr><td>Failed</td><td>" + failedTests + @"</td></tr>
                <tr><td>Error</td><td>" + errorTests + @"</td></tr>
                <tr><td>Total</td><td>" + totalTests + @"</td></tr>
            </tbody>
        </table>
    </div>

    <!-- Test Case List with Execution Time -->
    <div class='testcase-detail-table'>
        <h3>Test Cases List with Execution Time</h3>
       <table border='1'>
            <tr>
                <th>Total Test Runtime</th>
            </tr>
            <tr>");
            string exce = $"<td>{totalExecutionTime}</td>";
            htmlcontent1.AppendLine(exce);
            htmlcontent1.AppendLine(@"
            </tr>
        </table>

        <table border='1'>
            <tr>
                <th>Test Case Name</th>
                <th>Duration</th>
            </tr>
            ");
            foreach (var (testCase, execTime) in testExecutionTimes)
            {
                string sanitizedTest = testCase.Replace(" ", "_").Replace("-", "_");
                testDetailsTable = string.Join("", testDurations.Select(td =>
                    $"<tr><td>{td.Item1}</td><td>{td.Item2} seconds</td></tr>"
                ));
                
            }
            htmlcontent1.AppendLine(testDetailsTable);

            htmlcontent1.AppendLine(@"
        </table>
</div>

</div>

</body>
</html>");

            // **Save HTML Report**
            if (!Directory.Exists(report))
            {
                Directory.CreateDirectory(report);
            }
            File.WriteAllText(htmldashbard, htmlcontent1.ToString());
        }


        public static void jsforreport()
        {

            // Generate updated stats script
            string script = $@"
document.addEventListener('DOMContentLoaded', function() {{
    document.getElementById('totalTests').innerText = {UITestExtendReport.totalTests};
    document.getElementById('failedTests').innerText = {UITestExtendReport.failedTests};
    document.getElementById('errorTests').innerText = {UITestExtendReport.errorTests};
    document.getElementById('passedTests').innerText = {UITestExtendReport.passedTests};
}});
function showTestCaseDetails(testCaseId) {{
    console.log('Clicked test case:', testCaseId);
    document.querySelectorAll('.test-case-table').forEach(result => result.style.display = 'none');
    let selectedTestCase = document.getElementById(testCaseId);
    if (selectedTestCase) {{
        selectedTestCase.style.display = 'block';
    }} else {{
        console.error('Test case details not found for:', testCaseId);
    }}
}}";

            // Ensure the script is written correctly
            File.WriteAllText(updateStats, script);
        }
        // List to Store Test Information
        private static List<TestInfo> testInfos = new List<TestInfo>();

        // Log Test Result Method
        public static void LogTestResult(string testName, string status)
        {
            testEndTime = DateTime.Now;
            TimeSpan duration = testEndTime - testStartTime;

            string Name = TestContext.CurrentContext.Test.Name;
            double testDuration = duration.TotalSeconds;
            var testInfo = new TestInfo
            {
                Name = testName,
                Status = status,
                Duration = testDuration
            };
            testInfos.Add(testInfo);
        }



        public static void DurationEachTestCases()
        {
            testEndTime = DateTime.Now;
            TimeSpan duration = testEndTime - testStartTime;

            string testName = TestContext.CurrentContext.Test.Name;
            double testDuration = duration.TotalSeconds;

            testDurations.Add(new Tuple<string, double>(testName, testDuration)); // Store test name and duration

            // Log each test duration in Extent Report
            ///$"Test :  '{testName}' Duration: {testDuration} seconds";
        }

    }

    // Test Information Class
    public class TestInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public double Duration { get; set; } // Duration in seconds
    }

}
[Serializable]
public enum Status
{
    Pass,
    Fail,
    Fatal,
    Error,
    Warning,
    Info,
    Skip,
    Debug
}
