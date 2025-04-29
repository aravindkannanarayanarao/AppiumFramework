using Syncfusion.UITestHelpers.Core;
using Syncfusion.UITestHelpers.Appium;
using ImageMagick;
using Syncfusion.UITestHelpers.NUnit;

namespace Syncfusion.UITestHelpers.Screenshot
{
    public class MacScreenshotHelper : ScreenshotHelperBase
    {
        public MacScreenshotHelper(IApp app) : base(app, TestDevice.Mac) { }

        protected override byte[] TakeScreenshotInternal()
        {
            Thread.Sleep(500);

            var screenshot = _app.Screenshot() ?? throw new InvalidOperationException("Screenshot failed");

            //App.EnterFullScreen();
            //app.ExitFullScreen();
            Thread.Sleep(500);

            return screenshot;
        }
        protected override double diffPercentage1(string expectedPath, string actualPath, string diffPath, string name)
        {
            using var expected = new MagickImage(expectedPath);
            using var actual = new MagickImage(actualPath);

            // Create a new image to store the differences
            using var diff = new MagickImage();

            // Compare images
            var errorMetric = expected.Compare(actual, ErrorMetric.Absolute, diff);

            if (errorMetric > 0)
            {
                // Save the diff image
                diff.Format = MagickFormat.Png;
                diff.Write(diffPath);
                Console.WriteLine($"Differences found! Diff image saved at {diffPath}");

                UITestExtendReport.testException = $"Differences found! Diff image saved at {diffPath}";
            }
            else
            {
                Console.WriteLine("No differences found.");
                UITestExtendReport.testException = "No differences found.";
            }

            return errorMetric * 100; // 
        }
    }
}
