using OpenQA.Selenium;
using Syncfusion.UITestHelpers.Core;

namespace Syncfusion.UITestHelpers.Appium
{
	public class AppiumIOSVirtualKeyboardActions : AppiumVirtualKeyboardActions
	{
		public AppiumIOSVirtualKeyboardActions(AppiumApp app)
			: base(app)
		{
		}
		// Bug Dissmisskeyboard ruturn the keyboard action
		protected override CommandResponse DismissKeyboard(IDictionary<string, object> parameters)
		{
			try
			{
				if (_app.Driver.IsKeyboardShown())
				{
					_app.Driver.HideKeyboard("return");
				}
			}
			catch (InvalidElementStateException)
			{
				return CommandResponse.FailedEmptyResponse;
			}

			return CommandResponse.SuccessEmptyResponse;
		}

		protected override CommandResponse PressEnter(IDictionary<string, object> parameters)
		{
			try
			{
				if (_app.Driver.IsKeyboardShown())
				{
					_app.Driver.SwitchTo().ActiveElement().SendKeys(Keys.Return);
				}
			}
			catch (InvalidElementStateException)
			{
				return CommandResponse.FailedEmptyResponse;
			}

			return CommandResponse.SuccessEmptyResponse;
		}

		protected override CommandResponse PressVolumeDown(IDictionary<string, object> parameters)
		{
			try
			{
				// Press a physical button. The supported button name is home, volumedown, volumeup.
				// volumedown is only available for real devices.
				_app.Driver.ExecuteScript("mobile: pressButton", new Dictionary<string, object>
				{
					{ "name", "volumedown" },
				});

				return CommandResponse.SuccessEmptyResponse;
			}
			catch (Exception)
			{
				return CommandResponse.FailedEmptyResponse;
			}
		}

		protected override CommandResponse PressVolumeUp(IDictionary<string, object> parameters)
		{
			try
			{
				// Press a physical button. The supported button name is home, volumedown, volumeup.	
				// volumeup is only available for real devices.
				_app.Driver.ExecuteScript("mobile: pressButton", new Dictionary<string, object>
				{
					{ "name", "volumeup" },
				});

				return CommandResponse.SuccessEmptyResponse;
			}
			catch (Exception)
			{
				return CommandResponse.FailedEmptyResponse;
			}
		}
	}
}