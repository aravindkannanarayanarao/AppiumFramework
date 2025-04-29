using ImageMagick;
using Syncfusion.UITestHelpers.Core;
using Syncfusion.UITestHelpers.NUnit;
using System.Drawing.Imaging;
using System.Drawing;

namespace Syncfusion.UITestHelpers.Screenshot;

public class WindowsScreenshotHelper : ScreenshotHelperBase
{
    public WindowsScreenshotHelper(IApp app) : base(app, TestDevice.Windows) { }

    protected override int CropFromTop()
    {
        int cropFromTop = 0;
        return cropFromTop;
    }
    protected override int CropFromBottom()
    {
        int cropFromBottom = 0;
        return cropFromBottom;
    }

    protected override double diffPercentage1(string expectedPath, string actualPath, string diffPath, string name)
    {
        Bitmap img1 = new Bitmap(expectedPath);
        Bitmap img2 = new Bitmap(actualPath);
        Bitmap diffImg = new Bitmap(img1.Width, img1.Height);

        int differentPixels = 0;
        int totalPixels = img1.Width * img1.Height;
        const int blendStrength = 128; // 50% white overlay (0-255)
        const int white = 255;

        for (int x = 0; x < img1.Width; x++)
        {
            for (int y = 0; y < img1.Height; y++)
            {
                Color pixel1 = img1.GetPixel(x, y);
                Color pixel2 = img2.GetPixel(x, y);

                if (pixel1 != pixel2)
                {
                    // Mark differences in bright red
                    diffImg.SetPixel(x, y, Color.Red);
                    differentPixels++;
                }
                else
                {


                    // Semi-transparent light background
                    int r = (pixel1.R * (255 - blendStrength) + white * blendStrength) / 255;
                    int g = (pixel1.G * (255 - blendStrength) + white * blendStrength) / 255;
                    int b = (pixel1.B * (255 - blendStrength) + white * blendStrength) / 255;
                    diffImg.SetPixel(x, y, Color.FromArgb(255, r, g, b));

                }
            }
        }

        double differencePercentage = (differentPixels / (double)totalPixels) * 100;

        if (differentPixels > 0)
        {
            diffImg.Save(diffPath, ImageFormat.Png);
            Console.WriteLine($"Differences found! Diff image saved at {diffPath}");

            UITestExtendReport.testException = $"Differences found! Diff image saved at {diffPath}";
            //Assert.Fail();
        }
        else
        {
            Console.WriteLine("No differences found.");

            UITestExtendReport.testException = "No differences found.";
            //Assert.Pass();
        }

        return differencePercentage;
    }
}
