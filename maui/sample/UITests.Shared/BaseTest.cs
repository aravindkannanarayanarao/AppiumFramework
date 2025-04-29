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
using Syncfusion.UITestHelpers.Screenshot;

namespace UITests.Shared;

#if ANDROID
[TestFixture(TestDevice.Android)]
#elif IOS
[TestFixture(TestDevice.iOS)]
#elif MACOS
[TestFixture(TestDevice.Mac)]
#elif WINDOWS
[TestFixture(TestDevice.Windows)]
#endif
public abstract class BaseTest : UITestBase
{
    public BaseTest(TestDevice testDevice) : base(testDevice)
    {

    }

    public override IConfig GetTestConfig()
    {
        reportpath();
        var config = new Config();

        var appIdentifierKey = "AppId";
        // Note: an app with this ID has to be deployed to the emulator/device you want to run it on
        var appIdentifier = "com.companyname.synacfusioncontrols";
        var AppMain1 = "AppMain";
        var AppMain12 = "crc64ea26cc18b9cc2ea1";

        config.SetProperty(appIdentifierKey, appIdentifier);
        config.SetProperty(AppMain1, AppMain12);

        var appIdentifierKey11 = "AppName";
        // Note: a release build has to be done and the path to this .exe file should exist. Tweak this path if necessary
        var appIdentifier1 = "Synacfusioncontrols";

        config.SetProperty(appIdentifierKey11, appIdentifier1);

        // If the app ID is provided through an environment variable, like through CI, use that instead
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPID")))
        {
            appIdentifier = Environment.GetEnvironmentVariable("APPID");
        }

        //config.SetProperty(appIdentifierKey, appIdentifier);

        if (_testDevice == TestDevice.Mac)
        {
            var Macappname = "MacApp";
            var MacappnameID = "Synacfusioncontrols";
            config.SetProperty(Macappname, MacappnameID);

        }

        //config.SetProperty(appIdentifierKey, appIdentifier);

        if (_testDevice == TestDevice.iOS)
        {
            var appIdentifierKey10 = "iOSAppName";
            // Note: a release build has to be done and the path to this .exe file should exist. Tweak this path if necessary
            var appIdentifier10 = "com.companyname.synacfusioncontrols";

            config.SetProperty(appIdentifierKey10, appIdentifier10);
            // Note: this is passed down from the GitHub Action. If nothing is set, fall back to a default value below
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SIMID")))
            {
                config.SetProperty("Udid", Environment.GetEnvironmentVariable("SIMID"));
            }
            else
            {
                config.SetProperty("DeviceName", "iPhone 13");
                config.SetProperty("PlatformVersion", "15.2");
            }
        }


        return config;
    }


    public void reportpath()
    {
#if WINDOWS
        UITestExtendReport.report = @"..\..\..\report";
        UITestExtendReport.htmlhomepage = @"..\..\..\report\home.html";
        UITestExtendReport.htmldashbard = @"..\..\..\report\dashboard.html";
        UITestExtendReport.updateStats = @"..\..\..\report\updateStats.js";

#elif ANDROID || IOS || MACOS

        UITestExtendReport.report = @"../../../report";
        UITestExtendReport.htmlhomepage = @"../../../report/home.html";
        UITestExtendReport.htmldashbard = @"../../../report/dashboard.html";
        UITestExtendReport.updateStats = @"../../../report/updateStats.js";
#endif
    }


    //Common Methods

    public void Basicsbutton(string filename)

    {
        App.EnterText("editor", filename);
        App.Tap("btn");
        Thread.Sleep(1500);
    }
    public void Option()
    {
        Thread.Sleep(2000);
#if ANDROID
        App.TapCoordinates(1022f, 223f);
#else
        App.Tap("option");
#endif
        Thread.Sleep(1000);
    }
    public void TwoTap(string filename, string filename1)
    {
        App.Tap(filename);
        App.Tap(filename1);
    }
    public void coordinates()
    {
        App.AddPoint("Item1", 391f, 505f);
    }
    public void iTwoTap(string filename, string filename1)
    {
        App.Tap(filename);
        App.Tap(filename1);
#if iOS
            App.Tap("Done");
#endif
    }

    public void Tap(string filename, string filename1)
    {
        App.Tap(filename);
        App.EnterText(filename, filename1);
        App.DismissKeyboard();
    }
    public void Tap1(string filename, string filename1)
    {
        App.Tap(filename);
        App.ClearText(filename);
        App.EnterText(filename, filename1);
    }
    public void Coordinates(float x, float y, string imgName)
    {
#if ANDROID
        App.TapCoordinates(x, y);
        App.DismissKeyboard();
#else
        App.Tap("");
        App.DismissKeyboard();
#endif
        Thread.Sleep(1500);
        TakeAndCompareScreenshot(imgName);
    }

    public void pick(string filename, string filename1)
    {
        App.Tap(filename);
        App.Tap(filename1);
#if iOS
           App.Tap("Done");
#endif
    }

    public void TakeAndCompareScreenshot(string filename)
    {
#if ANDROID 
        var screenshotHelper = new AndroidScreenshotHelper(App);
        screenshotHelper.TakeAndCompareScreenshots(filename);
#elif WINDOWS

        var screenshotHelper = new WindowsScreenshotHelper(App);
        screenshotHelper.TakeAndCompareScreenshots(filename);
#elif IOS

        var screenshotHelper = new iOSScreenshotHelper(App);
        screenshotHelper.TakeAndCompareScreenshots(filename);
#elif MACOS

        var screenshotHelper = new MacScreenshotHelper(App);
        screenshotHelper.TakeAndCompareScreenshots(filename);
#endif
    }

    public void EnterText(string element, string text)
    {
#if ANDROID || WINDOWS || MACOS
        App.EnterText(element, text);
#elif IOS
        App.EnterTextiOS(element, text);
#endif

    }
}