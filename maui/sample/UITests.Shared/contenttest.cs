using NUnit.Framework;
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
        public void SemanticPropertiesGridNumericEntry_Tap()
        {
            App.Tap("numeric");
            App.WaitForElement("entry");
            App.Tap("entry");
            TakeAndCompareScreenshot("SemanticPropertiesGridNumericEntry_Tap");
        }
        [Test]
        public void SemanticPropertiesGridNumericEntry_Enter()
        {
            App.Tap("numeric");
            App.WaitForElement("entry");
            App.Tap("entry");
            App.EnterText("entry", "21");
            TakeAndCompareScreenshot("SemanticPropertiesGridNumericEntry_Enter");
        }

        [Test]
        public void SemanticPropertiesGridMaskedEdit_Tap()
        {
            App.Tap("mask");
            App.WaitForElement("Masked");
            App.Tap("Masked");
            TakeAndCompareScreenshot("SemanticPropertiesGridMaskedEdit_Tap");
        }
        [Test]
        public void SemanticPropertiesGridMaskedEdit_Enter()
        {
            App.Tap("mask");
            App.WaitForElement("Masked");
            App.Tap("Masked");
            App.EnterText("Masked", "21");
            TakeAndCompareScreenshot("SemanticPropertiesGridMaskedEdit_Enter");
        }
    }
}
