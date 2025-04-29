using NUnit.Framework;
using Syncfusion.UITestHelpers.Appium;
using Syncfusion.UITestHelpers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITests.Shared
{
    public class ControlTest : BaseTest
    {
        public ControlTest(TestDevice testDevice) : base(testDevice)
        {
            
        }
#region NumericEntry
        [Test]
        public void AutomationIDNumericEntry_Tap()
        {
            App.WaitForElement("Autoentry");
            App.Tap("Autoentry");
            TakeAndCompareScreenshot("AutomationIDNumericEntry_Tap");
        }
        [Test]
        public void AutomationIDNumericEntry_EnterText()
        {
            App.WaitForElement("Autoentry");
            App.Tap("Autoentry");
            App.EnterText("Autoentry", "21");
            TakeAndCompareScreenshot("AutomationIDNumericEntry_EnterText");
        }
        [Test]
        public void SemanticPropertiesNumericEntry_Tap()
        {
            App.WaitForElement("entry");
            App.Tap("entry");
            TakeAndCompareScreenshot("SemanticPropertiesNumericEntry_Tap");
        }
        [Test]
        public void SemanticPropertiesNumericEntry_Enter()
        {
            App.WaitForElement("entry");
            App.Tap("entry");
            App.EnterText("entry", "21");
            TakeAndCompareScreenshot("SemanticPropertiesNumericEntry_Enter");
        }
        #endregion
        #region MaskedEdit
        [Test]
        public void AutomationIDMaskedEdit_Tap()
        {
            App.WaitForElement("AutMasked");
            App.Tap("AutMasked");
            TakeAndCompareScreenshot("AutomationIDMaskedEdit_Tap");
        }
        [Test]
        public void AutomationIDMaskedEdit_EnterText()
        {
            App.WaitForElement("AutMasked");
            App.Tap("AutMasked");
            App.EnterText("AutMasked", "21");
            TakeAndCompareScreenshot("AutomationIDMaskedEdit_EnterText");
        }
        [Test]
        public void SemanticPropertiesMaskedEdit_Tap()
        {
            App.WaitForElement("Masked");
            App.Tap("Masked");
            TakeAndCompareScreenshot("SemanticPropertiesMaskedEdit_Tap");
        }
        [Test]
        public void SemanticPropertiesMaskedEdit_Enter()
        {
            App.WaitForElement("Masked");
            App.Tap("Masked");
            App.EnterText("Masked", "21");
            TakeAndCompareScreenshot("SemanticPropertiesMaskedEdit_Enter");
        }
        #endregion








        #region GridNumericEntry
        [Test]
        public void AutomationIDGridNumericEntry_Tap()
        {
            App.Tap("btngrid");
            App.WaitForElement("Autoentry");
            App.Tap("Autoentry");
            TakeAndCompareScreenshot("AutomationIDGridNumericEntry_Tap");
        }
        [Test]
        public void AutomationIDGridNumericEntry_EnterText()
        {
            App.Tap("btngrid");
            App.WaitForElement("Autoentry");
            App.Tap("Autoentry");
            App.EnterText("Autoentry", "21");
            TakeAndCompareScreenshot("AutomationIDGridNumericEntry_EnterText");
        }
        [Test]
        public void SemanticPropertiesGridNumericEntry_Tap()
        {
            App.Tap("btngrid");
            App.WaitForElement("entry");
            App.Tap("entry");
            TakeAndCompareScreenshot("SemanticPropertiesGridNumericEntry_Tap");
        }
        [Test]
        public void SemanticPropertiesGridNumericEntry_Enter()
        {
            App.Tap("btngrid");
            App.WaitForElement("entry");
            App.Tap("entry");
            App.EnterText("entry", "21");
            TakeAndCompareScreenshot("SemanticPropertiesGridNumericEntry_Enter");
        }
        #endregion
        #region GridMaskedEdit
        [Test]
        public void AutomationIDGridMaskedEdit_Tap()
        {
            App.Tap("btngrid");
            App.WaitForElement("AutMasked");
            App.Tap("AutMasked");
            TakeAndCompareScreenshot("AutomationIDGridMaskedEdit_Tap");
        }
        [Test]
        public void AutomationIDGridMaskedEdit_EnterText()
        {
            App.Tap("btngrid");
            App.WaitForElement("AutMasked");
            App.Tap("AutMasked");
            App.EnterText("AutMasked", "21");
            TakeAndCompareScreenshot("AutomationIDGridMaskedEdit_EnterText");
        }
        [Test]
        public void SemanticPropertiesGridMaskedEdit_Tap()
        {
            App.Tap("btngrid");
            App.WaitForElement("Masked");
            App.Tap("Masked");
            TakeAndCompareScreenshot("SemanticPropertiesGridMaskedEdit_Tap");
        }
        [Test]
        public void SemanticPropertiesGridMaskedEdit_Enter()
        {
            App.Tap("btngrid");
            App.WaitForElement("Masked");
            App.Tap("Masked");
            App.EnterText("Masked", "21");
            TakeAndCompareScreenshot("SemanticPropertiesGridMaskedEdit_Enter");
        }
        #endregion

        [Test]
        public void getpagesoucesautomationid()
        {
            Thread.Sleep(2000);
            var element = App.GetNativePageSource();
            Console.WriteLine(element);
        }
    }
}
