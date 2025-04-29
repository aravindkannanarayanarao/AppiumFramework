using NUnit.Framework;
using VisualTestUtils;

namespace Syncfusion.UITestHelpers.Screenshot
{

    public class VisualTestContext : ITestContext
    {
        public void AddTestAttachment(string filePath, string? description = null) =>
            TestContext.AddTestAttachment(filePath, description);
    }
}