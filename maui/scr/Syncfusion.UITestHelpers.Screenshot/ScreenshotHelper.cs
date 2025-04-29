using NUnit.Framework;
using Syncfusion.UITestHelpers.Appium;
using Syncfusion.UITestHelpers.Core;
using System.Diagnostics;
using VisualTestUtils;
using VisualTestUtils.MagickNet;
using System.Reflection;
using OpenQA.Selenium.Appium;
using Syncfusion.UITestHelpers.NUnit;
using System.Drawing.Imaging;
using System.Drawing;


namespace Syncfusion.UITestHelpers.Screenshot
{
#if ANDROID
[TestFixture(TestDevice.Android)]
#elif IOS
[TestFixture(TestDevice.iOS)]
#elif MACOS
[TestFixture(TestDevice.Mac)]
#elif WINDOWS
[TestFixture(TestDevice.Windows)]
#endif
    public abstract class  ScreenshotHelperBase 
    {
        private readonly string relativeTestRoot = Path.Combine("..", "..", "..", "Images");
        private readonly string relativeOutDirectory = Path.Combine("..", "..", "..", "Images", "snapshots-output");
        private readonly string relativeDiffDirectory = Path.Combine("..", "..", "..", "Images", "snapshots-diff");
        // Your common fields
        protected readonly VisualRegressionTester _visualRegressionTester;
        protected readonly IImageEditorFactory _imageEditorFactory;
        protected readonly VisualTestContext _visualTestContext;
        protected readonly IApp _app;
        protected readonly IAndroidApp _android;
        protected readonly IWindowsApp _Windows;
        protected readonly IIOSApp _iOS;
        protected readonly TestDevice _testDevice;

        public ScreenshotHelperBase(IApp app, TestDevice testDevice) 
        {
            _app = app;
            _testDevice = testDevice;
            _visualTestContext = new VisualTestContext();
            _imageEditorFactory = new MagickNetImageEditorFactory();

            string ciArtifactsDirectory = Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY");
            if (ciArtifactsDirectory != null)
                ciArtifactsDirectory = Path.Combine(ciArtifactsDirectory, "Controls.TestCases.Shared.Tests");

            string projectRoot = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!;
            _visualRegressionTester = new VisualRegressionTester(
                Path.Combine(projectRoot, "Images"),
                new MagickNetVisualComparer(),
                new MagickNetVisualDiffGenerator(),
                ciArtifactsDirectory);
        }

        public void TakeAndCompareScreenshots(string name)
        {
            byte[] screenshotBytes = TakeScreenshotInternal();
            var image = new ImageSnapshot(screenshotBytes, ImageSnapshotFormat.PNG);

            // Cropping logic based on platform
            IImageEditor editor = _imageEditorFactory.CreateImageEditor(image);
            (int width, int height) = editor.GetSize();

            int cropFromTop = CropFromTop();

            int cropFromBottom  = CropFromBottom();

            if (cropFromTop > 0 || cropFromBottom > 0)
            {
                editor.Crop(0, cropFromTop, width, height - cropFromTop - cropFromBottom);
                image = editor.GetUpdatedImage();
            }
            string projectRootDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!;
            string outputDirectory = Path.GetFullPath(Path.Combine(projectRootDirectory, relativeOutDirectory));
            if (!Directory.Exists(outputDirectory) )
            {
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine("Output and Diff directory created");
            }
            image.Save(outputDirectory, name);
            Compare(name, image);
        }

        private void Compare(string name, ImageSnapshot actualImage)
        {
            string projectRootDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!;

            // Convert relative paths to absolute paths
            string testRootDirectory = Path.GetFullPath(Path.Combine(projectRootDirectory, relativeTestRoot));
            string snapshotsDiffDirectory = Path.GetFullPath(Path.Combine(projectRootDirectory, relativeDiffDirectory));
            string outputDirectory = Path.GetFullPath(Path.Combine(projectRootDirectory, relativeOutDirectory));
            if (!Directory.Exists(outputDirectory) || !Directory.Exists(snapshotsDiffDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                Directory.CreateDirectory(snapshotsDiffDirectory);
                Console.WriteLine("Output and Diff directory created");
            }
            string expected = Path.Combine(testRootDirectory, "snapshots", $"{name}.png");
            string output = Path.Combine(relativeOutDirectory, $"{name}.png");
            string diffPath = Path.Combine(snapshotsDiffDirectory, $"{name}-diff.png");

            //string expected = Path.Combine(testRootDirectory, "snapshots", $"{name}.png");
            //string output = Path.Combine(testRootDirectory, "snapshots-output", $"{name}.png");
            //string diffPath = Path.Combine(testRootDirectory, "snapshots-diff", $"{name}-diff.png");

            //actualImage.Save(testRootDirectory, "snapshots-output");

            double diffPercentage = diffPercentage1(expected, output, diffPath, name);

            if (diffPercentage <= 0)
                Assert.Pass("Images match.");
            else
                Assert.Fail($"Images differ by {diffPercentage}%.");
        }

        protected virtual byte[] TakeScreenshotInternal()
        {
            return  _app.Screenshot() ?? throw new InvalidOperationException("Screenshot failed");
        }
        protected virtual int CropFromTop()
        {
            int cropFromTop = 0;
            return cropFromTop;
        }
        protected virtual int CropFromBottom()
        {
            int cropFromBottom = 0;
            return cropFromBottom;
        }
        protected virtual double diffPercentage1(string expected, string actual, string diffPath, string name)
        {
            double diff = 0.0 ;
            return diff;
        }
    }
}
