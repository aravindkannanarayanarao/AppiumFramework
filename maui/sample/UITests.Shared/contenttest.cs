using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Interactions.Internal;
using Syncfusion.UITestHelpers.Appium;
using Syncfusion.UITestHelpers.Core;

namespace UITests.Shared
{
    internal class contenttest : BaseTest
    {
        public contenttest(TestDevice testDevice) : base(testDevice)
        {
        }
        [Test]
        public void numeric()
        {
            App.WaitForElement("numeric");
            App.Tap("numeric");
            App.WaitForElement("entry");
            var element = App.GetNativePageSource();
            Console.WriteLine(element);
            App.EnterTextIntoCustomField("entry1", "67");
            TakeAndCompareScreenshot("numeric");
        }
        [Test]
        public void masked()
        {
            App.Tap("mask");
            App.WaitForElement("mask");
            App.Tap("entry");
            App.EnterTextIntoCustomField("Masked", "21");
            TakeAndCompareScreenshot("masked");
        }
        [Test]
        public void combobox()
        {
            App.Tap("combo");
            App.WaitForElement("combo");
            var element = App.GetNativePageSource();
            coordinates();
            App.TapByPointer("Item1");
            App.SelectFromComboBox("combo", 391f, 505f);
            TakeAndCompareScreenshot("combobox");
        }
        [Test]
        public void autocomplete()
        {
            App.Tap("auto");
            App.WaitForElement("autocomplete1");
            var element = App.GetNativePageSource();
            Console.WriteLine(element);
            App.Tap("autocomplete1");
            App.EnterTextIntoCustomField("autocomplete1", "fa");
            App.Tap("Facebook");
            TakeAndCompareScreenshot("autocomplete");
        }

    }
}
