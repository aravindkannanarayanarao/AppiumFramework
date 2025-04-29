using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Interactions;
using Syncfusion.UITestHelpers.Core;
using Syncfusion.UITestHelpers.NUnit;
using OpenQA.Selenium.Appium.MultiTouch;

namespace Syncfusion.UITestHelpers.Appium
{
    public static class HelperExtensions
    {
        static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
        private static void LogException(string methodName, Exception ex)
        {
            UITestExtendReport.testException = methodName + ex.ToString();
        }

        /// <summary>
        /// For desktop, this will perform a mouse click on the target element.
        /// For mobile, this will tap the element.
        /// This API works for all platforms whereas TapCoordinates currently doesn't work on Catalyst
        /// https://github.com/dotnet/maui/issues/19754
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void Tap(this IApp app, string element)
        {
            try
            {
                FindElement(app, element).Click();
            }
            catch (Exception ex)
            {
                LogException("Tap", ex);
                throw;
            }
        }

        /// <summary>
        /// For desktop, this will perform a mouse click on the target element.
        /// For mobile, this will tap the element.
        /// This API works for all platforms whereas TapCoordinates currently doesn't work on Catalyst
        /// https://github.com/dotnet/maui/issues/19754
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        public static void Tap(this IApp app, IQuery query)
        {
            try
            {
                app.FindElement(query).Tap();
            }
            catch (Exception ex)
            {
                LogException("Tap", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse click on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void Click(this IApp app, string element)
        {
            try
            {
                FindElement(app, element).Click();
            }
            catch (Exception ex)
            {
                LogException("Click", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse click on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        public static void Click(this IApp app, IQuery query)
        {
            try
            {
                app.FindElement(query).Click();
            }
            catch (Exception ex)
            {
                LogException("Click", ex);
                throw;
            }
        }

        public static void RightClick(this IApp app, string element)
        {

            try
            {
                var uiElement = FindElement(app, element);
                uiElement.Command.Execute("click", new Dictionary<string, object>()
            {
                { "element", uiElement },
                { "button", "right" }
            });
            }
            catch (Exception ex)
            {
                LogException("RightClick", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a down/press on the matched element, without a matching release
        /// </summary>
        /// <param name="app"></param>
        /// <param name="element"></param>
        public static void PressDown(this IApp app, string element)
        {

            try
            {
                var uiElement = FindElement(app, element);
                uiElement.Command.Execute("pressDown", new Dictionary<string, object>()
            {
                { "element", uiElement }
            });
            }
            catch (Exception ex)
            {
                LogException("PressDown", ex);
                throw;
            }
        }

        public static string? GetText(this IUIElement element)
        {
            try
            {
                var response = element.Command.Execute("getText", new Dictionary<string, object>()
            {
                { "element", element },
            });
                return (string?)response.Value;
            }
            catch (Exception ex)
            {
                LogException("GetText", ex);
                throw;
            }
        }

        public static string? ReadText(this IUIElement element)
            => element.GetText();

        public static T? GetAttribute<T>(this IUIElement element, string attributeName)
        {
            try
            {
                var response = element.Command.Execute("getAttribute", new Dictionary<string, object>()
            {
                { "element", element },
                { "attributeName", attributeName },
            });
                return (T?)response.Value;
            }
            catch (Exception ex)
            {
                LogException("GetAttribute", ex);
                throw;
            }
        }
        /// <summary>
        /// Return the current page source of the displayed view.
        /// Useful to inspect the current view hierarchy and its automation ids.
        /// </summary>
        public static string? GetNativePageSource(this IApp app)
        {
            var tree = (app as AppiumApp)?.Driver.PageSource;
            return tree;
        }

        /// <summary>
        /// Assert an element exists and is visible.
        /// </summary>
        public static bool DoesElementExist(this IApp app, string automationId)
        {
            var exists = false;

            var elements = app.FindElements(automationId);
            exists = elements.Count > 0;

            return exists && elements.First().IsDisplayed();
        }

        /// <summary>
        /// Assert an element found by text exists and is visible.
        /// </summary>
        public static IUIElement? FindVisibleElementByText(this IApp app, string text, int timeoutInSeconds = 3)
        {
            string beforeMessage = $"Find visible element by text: '{text}' - {DateTime.Now.ToString("HH:mm:ss")}";
            Console.WriteLine(beforeMessage);

            var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < timeout)
            {
                var elementIsVisible = app.FindElementByText(text)?.IsDisplayed();
                if (elementIsVisible is not null && elementIsVisible.Value)
                {
                    var element = app.FindElementByText(text);
                    string afterMessage = $"Found visible element by text: {text} - {DateTime.Now.ToString("HH:mm:ss")}";
                    Console.WriteLine(afterMessage);
                    return element;
                }
                Thread.Sleep(100);
            }
            stopwatch.Stop();
            string notVisibleMessage = $"Element with text: {text} not visible within timeout - {DateTime.Now.ToString("HH:mm:ss")}";
            Console.WriteLine(notVisibleMessage);
            return null;
        }


        /// <summary>
        /// Tap then wait a short period of time for other elements to load.
        /// /// <param name="delayInMilliseconds">Amount of time to wait in milliseconds.</param>
        /// </summary>
        public static void TapWithDelay(this IUIElement element, int delayInMilliseconds = 500)
        {
            element.Tap();
            Task.Delay(delayInMilliseconds).Wait();
        }

        /// <summary>
        /// Tap an element by its text.
        /// </summary>
        public static void TapByText(this IApp app, string text)
        {
            var driver = (app as AppiumApp)?.Driver;
            if (driver == null)
            {
                throw new InvalidOperationException($"Driver was null");
            }

            // Find the element by XPath
            var element = (app.FindElementByText(text) as AppiumDriverElement)?.AppiumElement;
            if (element == null)
            {
                throw new EntryPointNotFoundException($"Element with XPath '{text}' not found");
            }

            // Get the element's size and location
            var elementLocation = element.Location;
            var elementSize = element.Size;

            // Calculate the center of the element
            int centerX = elementLocation.X + (elementSize.Width / 2);
            int centerY = elementLocation.Y + (elementSize.Height / 2);

            // Create W3C actions
            var actions = new PointerInputDevice(PointerKind.Touch);
            var sequence = new ActionSequence(actions, 0);

            // Perform touch tap at the center of the element
            sequence.AddAction(actions.CreatePointerMove(CoordinateOrigin.Viewport, centerX, centerY, TimeSpan.Zero));
            sequence.AddAction(actions.CreatePointerDown(MouseButton.Left));
            sequence.AddAction(actions.CreatePointerUp(MouseButton.Left));

            // Execute the tap action
            driver.PerformActions(new List<ActionSequence> { sequence });
        }


        public static Rectangle GetRect(this IUIElement element)
        {
            try
            {
                var response = element.Command.Execute("getRect", new Dictionary<string, object>()
            {
                { "element", element },
            });

                if (response?.Value != null)
                {
                    return (Rectangle)response.Value;
                }

                throw new InvalidOperationException($"Could not get Rect of element");
            }
            catch (Exception ex)
            {
                LogException("GetRect", ex);
                throw;
            }
        }

        /// <summary>
        /// Determine if a form or form-like element (checkbox, select, etc...) is selected.
        /// </summary>
        /// <param name="element">Target Element.</param>
        /// <returns>Whether the element is selected (boolean).</returns>
        public static bool IsSelected(this IUIElement element)
        {
            try
            {
                var response = element.Command.Execute("getSelected", new Dictionary<string, object>()
            {
                { "element", element },
            });

                if (response?.Value != null)
                {
                    return (bool)response.Value;
                }

                throw new InvalidOperationException($"Could not get Selected of element");
            }
            catch (Exception ex)
            {
                LogException("IsSelected", ex);
                throw;
            }
        }

        /// <summary>
        /// Determine if an element is currently displayed.
        /// </summary>
        /// <param name="element">Target Element.</param>
        /// <returns>Whether the element is displayed (boolean).</returns>
        public static bool IsDisplayed(this IUIElement element)
        {
            try
            {
                var response = element.Command.Execute("getDisplayed", new Dictionary<string, object>()
            {
                { "element", element },
            });

                if (response?.Value != null)
                {
                    return (bool)response.Value;
                }

                throw new InvalidOperationException($"Could not get Displayed of element");
            }
            catch (Exception ex)
            {
                LogException("IsDisplayed", ex);
                throw;
            }
        }

        /// <summary>
        /// Determine if an element is currently enabled.
        /// </summary>
        /// <param name="element">Target Element.</param>
        /// <returns>Whether the element is enabled (boolean).</returns>
        public static bool IsEnabled(this IUIElement element)
        {

            try
            {
                var response = element.Command.Execute("getEnabled", new Dictionary<string, object>()
            {
                { "element", element },
            });

                if (response?.Value != null)
                {
                    return (bool)response.Value;
                }

                throw new InvalidOperationException($"Could not get Enabled of element");
            }
            catch (Exception ex)
            {
                LogException("IsEnabled", ex);
                throw;
            }
        }

        /// <summary>
        /// Enters text into a custom input control (e.g., Syncfusion SfNumericTextBox) by locating 
        /// the associated editable field (EditText) nested inside a parent container identified 
        /// by its accessibility ID (content-desc).
        /// </summary>
        /// <param name="accessibilityId">The accessibility ID (SemanticProperties.Description) of the parent container view.</param>
        /// <param name="value">The text value to enter into the EditText field.</param>
        /// <exception cref="NoSuchElementException">Thrown if the container or EditText element is not found.</exception>
        /// <exception cref="InvalidElementStateException">Thrown if the target element cannot accept input.</exception>
        /// <exception cref="InvalidElementStateException">Cannot set the element to 'T'. Did you interact with the correct element?.</exception>
        public static void EnterTextIntoCustomField(this IApp app, string accessibilityId, string value)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                var container = driver.FindElement(MobileBy.AccessibilityId(accessibilityId));
                var input = container.FindElement(By.ClassName("android.widget.EditText"));
                input.Clear();
                input.SendKeys(value);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }

        /// <summary>
        /// Selects a specified item from a ComboBox control by first tapping the dropdown button
        /// and then choosing the desired value from the displayed list.
        /// </summary>
        /// <param name="accessibilityId">The accessibility ID of the ComboBox container.</param>
        /// <param name="valueToSelect">The visible text of the item to select from the dropdown list.</param>

        public static void SelectFromComboBox(this IApp app, string accessibilityId, float x,float y)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                var comboContainer = driver.FindElement(MobileBy.AccessibilityId(accessibilityId));

                // Tap the dropdown button (usually a sibling of EditText)
                var dropdownButton = comboContainer.FindElement(By.XPath(".//*[contains(@content-desc, 'Drop down')]"));
                dropdownButton.Click();
                Thread.Sleep(2000);
                app.TapCoordinates(x, y);

            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        } public static void SelectFromComboBox(this IApp app, string accessibilityId)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                var comboContainer = driver.FindElement(MobileBy.AccessibilityId(accessibilityId));

                // Tap the dropdown button (usually a sibling of EditText)
                var dropdownButton = comboContainer.FindElement(By.XPath(".//*[contains(@content-desc, 'Drop down')]"));
                dropdownButton.Click();

            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }
        /// <summary>
        /// Selects a specified item from a ComboBox control by first tapping the dropdown button
        /// and then choosing the desired value from the displayed list.
        /// </summary>
        /// <param name="accessibilityId">The accessibility ID of the ComboBox container.</param>
        /// <param name="valueToSelect">The visible text of the item to select from the dropdown list.</param>

        public static void SelectFromComboBox(this IApp app, string accessibilityId,string SearchValue, float x,float y)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                var comboContainer = driver.FindElement(MobileBy.AccessibilityId(accessibilityId));

                // Tap the dropdown button (usually a sibling of EditText)
                var input = comboContainer.FindElement(By.ClassName("android.widget.EditText"));
                input.Click();
                input.SendKeys(SearchValue);
                app.TapCoordinates(x, y);

            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }
        /// <summary>
        /// Types into an AutoComplete input field and selects the matching suggestion from the dropdown list.
        /// Useful for controls that display suggestions dynamically based on typed text.
        /// </summary>
        /// <param name="accessibilityId">The accessibility ID of the AutoComplete input field.</param>
        /// <param name="valueToType">The partial or full text to type into the input field to trigger suggestions.</param>
        /// <param name="valueToSelect">The visible text of the suggestion to select from the dropdown list.</param>
        public static void SelectFromAutoComplete(this IApp app, string accessibilityId, string valueToType, float x, float y)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                var input = driver.FindElement(MobileBy.AccessibilityId(accessibilityId));
                var inputview = input.FindElement(By.ClassName("android.widget.EditText"));
                inputview.Clear();
                inputview.SendKeys(valueToType);

                // Wait for suggestion dropdown and tap the correct item
                app.TapCoordinates(x, y);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }



        /// <summary>
        /// Enters text into the element identified by the query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        /// <param name="text">The text to enter.</param>
        public static void EnterText(this IApp app, string element, string text)
        {
            try
            {
                var appElement = app.FindElement(element);

                app.EnterText(appElement, text);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }
        public static void EnterTextiOS(this IApp app, string element, string text)
        {
            try
            {
                var appElement = app.FindElement(element);

                app.EnterTextiOS(appElement, text);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }

        /// <summary>
        /// Enters text into the element identified by the query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        public static void EnterText(this IApp app, IQuery query, string text)
        {
            try
            {
                var appElement = app.FindElement(query);

                app.EnterText(appElement, text);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }
        public static void EnterTextiOS(this IApp app, IQuery query, string text)
        {
            try
            {
                var appElement = app.FindElement(query);

                app.EnterTextiOS(appElement, text);
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }

        internal static void EnterText(this IApp app, IUIElement? element, string text)
        {
            try
            {
                if (element is not null)
                {
                    element.SendKeys(text);
                    app.DismissKeyboard();
                }
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }
        internal static void EnterTextiOS(this IApp app, IUIElement? element, string text)
        {
            try
            {
                if (element is not null)
                {
                    element.SendKeys(text);
                }
            }
            catch (Exception ex)
            {
                LogException("EnterText", ex);
                throw;
            }
        }

        /// <summary>
        /// Hides soft keyboard if present.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void DismissKeyboard(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("dismissKeyboard", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("DismissKeyboard", ex);
                throw;
            }
        }

        /// <summary>
        /// Whether or not the soft keyboard is shown.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <returns>true if the soft keyboard is shown; otherwise, false.</returns>
        public static bool IsKeyboardShown(this IApp app)
        {
            try
            {
                var response = app.CommandExecutor.Execute("isKeyboardShown", ImmutableDictionary<string, object>.Empty);
                var responseValue = response?.Value ?? false;
                return (bool)responseValue;
            }
            catch (Exception ex)
            {
                LogException("IsKeyboardShown", ex);
                throw;
            }
        }

        /// <summary>
        /// (Android Only) Sends a device key event with meta state.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="keyCode"> Code for the key pressed on the Android device</param>
        /// <param name="metastate">metastate for the key press</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SendKeys(this IApp app, int keyCode, int metastate = 0)
        {
            try
            {
                if (app is not AppiumApp aaa)
                {
                    throw new InvalidOperationException($"SendKeys is only supported on AppiumApp");
                }

                if (aaa.Driver is ISendsKeyEvents ske)
                {
                    ske.PressKeyCode(keyCode, metastate);
                    return;
                }

                throw new InvalidOperationException($"SendKeys is not supported on {aaa.Driver}");
            }
            catch (Exception ex)
            {
                LogException("SendKeys", ex);
                throw;
            }
        }

        /// <summary>
        /// Clears text from the currently focused element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void ClearText(this IApp app, string element)
        {
            try
            {
                FindElement(app, element).Clear();
            }
            catch (Exception ex)
            {
                LogException("ClearText", ex);
                throw;
            }
        }

        /// <summary>
        /// Clears text from the currently focused element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        public static void ClearText(this IApp app, IQuery query)
        {
            try
            {
                app.FindElement(query).Clear();
            }
            catch (Exception ex)
            {
                LogException("ClearText", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse click on the matched element.
        /// </summary>
        /// <param name="element">Target Element.</param>
        public static void Click(this IUIElement element)
        {
            try
            {
                element.Command.Execute("click", new Dictionary<string, object>()
            {
                { "element", element }
            });
            }
            catch (Exception ex)
            {
                LogException("Click", ex);
                throw;
            }
        }

        /// <summary>
        /// For desktop, this will perform a mouse click on the target element.
        /// For mobile, this will tap the element.
        /// This API works for all platforms whereas TapCoordinates currently doesn't work on Catalyst
        /// https://github.com/dotnet/maui/issues/19754
        /// </summary>
        /// <param name="element">Target Element.</param>
        public static void Tap(this IUIElement element)
        {
            try
            {
                element.Command.Execute("tap", new Dictionary<string, object>()
            {
                { "element", element }
            });
            }
            catch (Exception ex)
            {
                LogException("Tap", ex);
                throw;
            }
        }

        public static void SendKeys(this IUIElement element, string text)
        {
            try
            {
                element.Command.Execute("sendKeys", new Dictionary<string, object>()
            {
                { "element", element },
                { "text", text }
            });
            }
            catch (Exception ex)
            {
                LogException("SendKeys", ex);
                throw;
            }
        }

        public static void Clear(this IUIElement element)
        {
            try
            {
                element.Command.Execute("clear", new Dictionary<string, object>()
            {
                { "element", element },
            });
            }
            catch (Exception ex)
            {
                LogException("Clear", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse double click on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void DoubleClick(this IApp app, string element)
        {
            try
            {
                var elementToDoubleClick = FindElement(app, element);
                app.CommandExecutor.Execute("doubleClick", new Dictionary<string, object>
            {
                { "element", elementToDoubleClick },
            });
            }
            catch (Exception ex)
            {
                LogException("DoubleClick", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse double click on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate to double click.</param>
        /// <param name="y">The y coordinate to double click.</param>
        public static void DoubleClickCoordinates(this IApp app, float x, float y)
        {
            try
            {
                app.CommandExecutor.Execute("doubleClickCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y }
            });
            }
            catch (Exception ex)
            {
                LogException("DoubleClickCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs two quick tap / touch gestures on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void DoubleTap(this IApp app, string element)
        {
            try
            {
                var elementToDoubleTap = app.FindElement(element);
                app.DoubleTap(elementToDoubleTap);
            }
            catch (Exception ex)
            {
                LogException("DoubleTap", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs two quick tap / touch gestures on the matched element by 'query'.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query"></param>
        public static void DoubleTap(this IApp app, IQuery query)
        {
            try
            {
                var elementToDoubleTap = app.FindElement(query);
                app.DoubleTap(elementToDoubleTap);
            }
            catch (Exception ex)
            {
                LogException("DoubleTap", ex);
                throw;
            }
        }

        internal static void DoubleTap(this IApp app, IUIElement? element)
        {
            if (element is not null)
            {
                app.CommandExecutor.Execute("doubleTap", new Dictionary<string, object>
                {
                    { "element", element },
                });
            }
        }

        /// <summary>
        /// Performs two quick tap / touch gestures on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate to double tap.</param>
        /// <param name="y">The y coordinate to double tap.</param>
        public static void DoubleTapCoordinates(this IApp app, float x, float y)
        {
            try
            {
                app.CommandExecutor.Execute("doubleTapCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y }
            });
            }
            catch (Exception ex)
            {
                LogException("DoubleTapCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a long mouse click on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void LongPress(this IApp app, string element)
        {
            try
            {
                var elementToLongPress = FindElement(app, element);
                app.CommandExecutor.Execute("longPress", new Dictionary<string, object>
            {
                { "element", elementToLongPress },
            });
            }
            catch (Exception ex)
            {
                LogException("LongPress", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a continuous touch gesture on the matched element.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Target Element.</param>
        public static void TouchAndHold(this IApp app, string element)
        {
            try
            {
                var elementToTouchAndHold = app.FindElement(element);
                app.TouchAndHold(elementToTouchAndHold);
            }
            catch (Exception ex)
            {
                LogException("TouchAndHold", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a continuous touch gesture on an element matched by 'query'.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        public static void TouchAndHold(this IApp app, IQuery query)
        {
            try
            {
                var elementToTouchAndHold = app.FindElement(query);

                app.TouchAndHold(elementToTouchAndHold);
            }
            catch (Exception ex)
            {
                LogException("TouchAndHold", ex);
                throw;
            }
        }

        internal static void TouchAndHold(this IApp app, IUIElement? element)
        {
            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("touchAndHold", new Dictionary<string, object>
                {
                    { "element", element },
                });
                }
            }
            catch (Exception ex)
            {
                LogException("TouchAndHold", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a continuous touch gesture on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate to touch.</param>
        /// <param name="y">The y coordinate to touch.</param>
        public static void TouchAndHoldCoordinates(this IApp app, float x, float y)
        {
            try
            {
                app.CommandExecutor.Execute("touchAndHoldCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y }
            });
            }
            catch (Exception ex)
            {
                LogException("TouchAndHoldCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a long touch on an item, followed by dragging the item to a second item and dropping it.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="dragSource">Element to be dragged.</param>
        /// <param name="dragTarget">Element to be dropped.</param>
        public static void DragAndDrop(this IApp app, string dragSource, string dragTarget)
        {
            try
            {
                var dragSourceElement = FindElement(app, dragSource);
                var targetSourceElement = FindElement(app, dragTarget);

                app.DragAndDrop(dragSourceElement, targetSourceElement);
            }
            catch (Exception ex)
            {
                LogException("DragAndDrop", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a long touch on an item, followed by dragging the item to a second item and dropping it.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="dragSource">Represents the query that identify a source element by parameters such as type, text it contains or identifier.</param>
        /// <param name="dragTarget">Represents the query that identify a target element by parameters such as type, text it contains or identifier.</param>
        public static void DragAndDrop(this IApp app, IQuery dragSource, IQuery dragTarget)
        {
            try
            {
                var dragSourceElement = app.FindElement(dragSource);
                var targetSourceElement = app.FindElement(dragTarget);
                app.DragAndDrop(dragSourceElement, targetSourceElement);
            }
            catch (Exception ex)
            {
                LogException("DragAndDrop", ex);
                throw;
            }
        }

        internal static void DragAndDrop(this IApp app, IUIElement? dragSourceElement, IUIElement? targetSourceElement)
        {
            try
            {
                if (dragSourceElement is not null && targetSourceElement is not null)
                {
                    app.CommandExecutor.Execute("dragAndDrop", new Dictionary<string, object>
                {
                    { "sourceElement", dragSourceElement },
                    { "destinationElement", targetSourceElement }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("DragAndDrop", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a pinch gestures on the matched element to zoom the view in. 
        /// If multiple elements are matched, the first one will be used.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Element to zoom in.</param>
        /// <param name="duration">The TimeSpan duration of the pinch gesture.</param>
        public static void PinchToZoomIn(this IApp app, string element, TimeSpan? duration = null)
        {
            try
            {
                var elementToPinchToZoomIn = app.FindElement(element);

                app.CommandExecutor.Execute("pinchToZoomIn", new Dictionary<string, object>
            {
                { "element", elementToPinchToZoomIn },
                { "duration", duration ?? TimeSpan.FromSeconds(1) }
            });
            }
            catch (Exception ex)
            {
                LogException("PinchToZoomIn", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a pinch gestures to zoom the view in on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate of the center of the pinch.</param>
        /// <param name="y">The y coordinate of the center of the pinch.</param>
        /// <param name="duration">The TimeSpan duration of the pinch gesture.</param>
        public static void PinchToZoomInCoordinates(this IApp app, float x, float y, TimeSpan? duration = null)
        {
            try
            {
                app.CommandExecutor.Execute("pinchToZoomInCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y },
                { "duration", duration ?? TimeSpan.FromSeconds(1) }
            });
            }
            catch (Exception ex)
            {
                LogException("PinchToZoomInCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a pinch gestures on the matched element to zoom the view out. 
        /// If multiple elements are matched, the first one will be used.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="element">Element to zoom in.</param>
        /// <param name="duration">The TimeSpan duration of the pinch gesture.</param>
        public static void PinchToZoomOut(this IApp app, string element, TimeSpan? duration = null)
        {
            try
            {
                var elementToPinchToZoomOut = app.FindElement(element);

                app.CommandExecutor.Execute("pinchToZoomOut", new Dictionary<string, object>
            {
                { "element", elementToPinchToZoomOut },
                { "duration", duration ?? TimeSpan.FromSeconds(1) }
            });
            }
            catch (Exception ex)
            {
                LogException("PinchToZoomOut", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a pinch gestures to zoom the view out on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate of the center of the pinch.</param>
        /// <param name="y">The y coordinate of the center of the pinch.</param>
        /// <param name="duration">The TimeSpan duration of the pinch gesture.</param>
        public static void PinchToZoomOutCoordinates(this IApp app, float x, float y, TimeSpan? duration = null)
        {
            try
            {
                app.CommandExecutor.Execute("pinchToZoomOutCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y },
                { "duration", duration ?? TimeSpan.FromSeconds(1) }
            });
            }
            catch (Exception ex)
            {
                LogException("PinchToZoomOutCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls within a specified element until a target element is visible or a timeout occurs.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="toElementId">Specify what element to scroll within.</param>
        /// <param name="down">Whether scrolls should be down or up.</param>
        /// <param name="elementToFind">The ID of the element to find.</param>
        /// <param name="scrollingElement">The ID of the element to scroll within.</param>
        /// <param name="down">Indicates whether to scroll down (true) or up (false).</param>
        /// <param name="horizontalStartPoint">Optional horizontal start point for the scroll.</param>
        /// <param name="percentToScroll">The percentage of the element to scroll with each gesture.</param>
        /// <param name="timeoutInSeconds">The maximum time to wait for the element to become visible.</param>
        /// <exception cref="NotFoundException">Thrown if the element is not visible within the timeout period.</exception>
        public static void ScrollUntilVisible(this IApp app, string elementToFind, string scrollingElement, bool down = true, int? horizontalStartPoint = null, double percentToScroll = 0.5, int timeoutInSeconds = 30)
        {
            try
            {
                var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                var stopwatch = Stopwatch.StartNew();

                while (stopwatch.Elapsed < timeout)
                {
                    var element = app.FindElement(elementToFind);
                    if (element != null && element.IsDisplayed())
                    {
                        return;
                    }

                    Scroll(app, scrollingElement, down, percentToScroll, horizontalStartPoint);
                }

                stopwatch.Stop();
                throw new NotFoundException($"Element with ID '{elementToFind}' not visible within timeout.");

            }
            catch (Exception ex)
            {
                LogException("ScrollTo", ex);
                throw;
            }
        }


        /// <summary>
        /// Scrolls within a specified element by a given percentage.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="scrollingElement">The ID of the element to scroll within.</param>
        /// <param name="down">Indicates whether to scroll down (true) or up (false).</param>
        /// <param name="percentToScroll">The percentage of the element to scroll with each gesture.</param>
        /// <param name="horizontalStartPoint">Optional horizontal start point for the scroll.</param>
        /// <exception cref="NotFoundException">Thrown if the driver is null.</exception>
        private static void Scroll(IApp app, string scrollingElement, bool down, double percentToScroll, int? horizontalStartPoint = null)
        {
            try
            {
                var driver = (app as AppiumApp)?.Driver;
                if (driver == null)
                {
                    throw new NotFoundException($"Driver was null");
                }

                // Find the scrollable container element
                var scrollableElement = driver.FindElement(By.Id(scrollingElement));

                // Get the element's size and location
                var elementLocation = scrollableElement.Location;
                var elementSize = scrollableElement.Size;

                // Calculate scroll start and end positions within the element
                int startX = horizontalStartPoint is not null ? elementLocation.X + horizontalStartPoint.Value : elementLocation.X + (elementSize.Width / 2);
                int startY = elementLocation.Y + (int)(elementSize.Height * (down ? 0.8 : 0.2)); // Start from 80% down or 20% up inside the element
                int endY = elementLocation.Y + (int)(elementSize.Height * (down ? (1 - percentToScroll) : percentToScroll));

                // Create W3C actions
                var actions = new PointerInputDevice(PointerKind.Touch);
                var sequence = new ActionSequence(actions, 0);

                // Perform touch swipe within the element
                sequence.AddAction(actions.CreatePointerMove(CoordinateOrigin.Viewport, startX, startY, TimeSpan.Zero));
                sequence.AddAction(actions.CreatePointerDown(MouseButton.Left));
                sequence.AddAction(actions.CreatePointerMove(CoordinateOrigin.Viewport, startX, endY, TimeSpan.FromMilliseconds(500)));
                sequence.AddAction(actions.CreatePointerUp(MouseButton.Left));

                // Execute the scroll action
                driver.PerformActions([sequence]);
            }

            catch (Exception ex)
            {
                LogException("ScrollTo", ex);
                throw;
            }
        }

        /// <summary>
        /// Return the currently presented alert or action sheet.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static IUIElement? GetAlert(this IApp app)
        {
            try
            {
                return app.GetAlerts().FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogException("GetAlert", ex);
                throw;
            }
        }

        /// <summary>
        /// Return the currently presented alerts or action sheets.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static IReadOnlyCollection<IUIElement> GetAlerts(this IApp app)
        {
            try
            {
                var result = app.CommandExecutor.Execute("getAlerts", ImmutableDictionary<string, object>.Empty);
                return (IReadOnlyCollection<IUIElement>?)result.Value ?? Array.Empty<IUIElement>();
            }
            catch (Exception ex)
            {
                LogException("GetAlerts", ex);
                throw;
            }
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        /// <param name="alertElement">The element that represents the alert or action sheet.</param>
        public static void DismissAlert(this IUIElement alertElement)
        {
            try
            {
                alertElement.Command.Execute("dismissAlert", new Dictionary<string, object>
                {
                    ["element"] = alertElement
                });
            }
            catch (Exception ex)
            {
                LogException("DismissAlert", ex);
                throw;
            }
        }

        /// <summary>
        /// Return the buttons in the alert or action sheet.
        /// </summary>
        /// <param name="alertElement">The element that represents the alert or action sheet.</param>
        public static IReadOnlyCollection<IUIElement> GetAlertButtons(this IUIElement alertElement)
        {
            try
            {
                var result = alertElement.Command.Execute("getAlertButtons", new Dictionary<string, object>
                {
                    ["element"] = alertElement
                });
                return (IReadOnlyCollection<IUIElement>?)result.Value ?? Array.Empty<IUIElement>();
            }
            catch (Exception ex)
            {
                LogException("GetAlertButtons", ex);
                throw;
            }
        }

        /// <summary>
        /// Return the text messages in the alert or action sheet.
        /// </summary>
        /// <param name="alertElement">The element that represents the alert or action sheet.</param>
        public static IReadOnlyCollection<string> GetAlertText(this IUIElement alertElement)
        {
            try
            {
                var result = alertElement.Command.Execute("getAlertText", new Dictionary<string, object>
                {
                    ["element"] = alertElement
                });
                return (IReadOnlyCollection<string>?)result.Value ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                LogException("GetAlertText", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is found. 
        /// Throws a TimeoutException if no element is found within the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Target Element.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        /// <param name="postTimeout">The final TimeSpan to wait after the element has been found.</param>
        public static IUIElement WaitForElement(this IApp app, string marked, string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IUIElement result() => FindElement(app, marked);
                var results = WaitForAtLeastOne(result, timeoutMessage, timeout, retryFrequency);

                return results;
            }
            catch (Exception ex)
            {
                LogException("WaitForElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until any matching element is found. 
        /// Throws a TimeoutException if no element is found within the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Collection of target Elements.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        /// <param name="postTimeout">The final TimeSpan to wait after the element has been found.</param>
        public static IUIElement WaitForAnyElement(this IApp app, string[] marked, string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IUIElement result() => FindAnyElement(app, marked);
                var results = WaitForAtLeastOne(result, timeoutMessage, timeout, retryFrequency);

                return results;
            }
            catch (Exception ex)
            {
                LogException("WaitForAnyElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is found. 
        /// Throws a TimeoutException if no element is found within the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        /// <param name="postTimeout">The final TimeSpan to wait after the element has been found.</param>
        public static IUIElement WaitForElement(this IApp app, IQuery query, string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IUIElement result() => app.FindElement(query);
                var results = WaitForAtLeastOne(result, timeoutMessage, timeout, retryFrequency);

                return results;
            }
            catch (Exception ex)
            {
                LogException("WaitForElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is found. 
        /// Throws a TimeoutException if no element is found within the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Entry point for the fluent API to specify the element.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        public static IUIElement WaitForElement(
            this IApp app,
            Func<IUIElement?> query,
            string? timeoutMessage = null,
            TimeSpan? timeout = null,
            TimeSpan? retryFrequency = null)
        {
            try
            {
                var results = Wait(query, i => i != null, timeoutMessage, timeout, retryFrequency);

                return results;
            }
            catch (Exception ex)
            {
                LogException("WaitForElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is no longer found. 
        /// Throws a TimeoutException if the element is visible at the end of the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Target Element.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        /// <param name="postTimeout">The final TimeSpan to wait after the element has been found.</param>
        public static void WaitForNoElement(this IApp app, string marked, string timeoutMessage = "Timed out waiting for no element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IUIElement result() => app.FindElement(marked);
                WaitForNone(result, timeoutMessage, timeout, retryFrequency);
            }
            catch (Exception ex)
            {
                LogException("WaitForNoElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is no longer found. 
        /// Throws a TimeoutException if the element is visible at the end of the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        /// <param name="postTimeout">The final TimeSpan to wait after the element has been found.</param>
        public static void WaitForNoElement(this IApp app, IQuery query, string timeoutMessage = "Timed out waiting for no element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IUIElement result() => app.FindElement(query);
                WaitForNone(result, timeoutMessage, timeout, retryFrequency);
            }
            catch (Exception ex)
            {
                LogException("WaitForNoElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Wait function that will repeatedly query the app until a matching element is no longer found. 
        /// Throws a TimeoutException if the element is visible at the end of the time limit.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Entry point for the fluent API to specify the element.</param>
        /// <param name="timeoutMessage">The message used in the TimeoutException.</param>
        /// <param name="timeout">The TimeSpan to wait before failing.</param>
        /// <param name="retryFrequency">The TimeSpan to wait between each query call to the app.</param>
        public static void WaitForNoElement(
            this IApp app,
            Func<IUIElement?> query,
            string? timeoutMessage = null,
            TimeSpan? timeout = null,
            TimeSpan? retryFrequency = null)
        {
            try
            {
                Wait(query, i => i is null, timeoutMessage, timeout, retryFrequency);
            }
            catch (Exception ex)
            {
                LogException("WaitForNoElement", ex);
                throw;
            }
        }

        public static IUIElement WaitForFirstElement(this IApp app, string marked, string timeoutMessage = "Timed out waiting for element...", TimeSpan? timeout = null, TimeSpan? retryFrequency = null, TimeSpan? postTimeout = null)
        {
            try
            {
                IReadOnlyCollection<IUIElement> elements = FindElements(app, marked);

                if (elements is not null && elements.Count > 0)
                {
                    IUIElement firstElement() => elements.First();

                    var result = Wait(firstElement, i => i != null, timeoutMessage, timeout, retryFrequency);

                    return result;
                }

                return WaitForElement(app, marked, timeoutMessage, timeout, retryFrequency, postTimeout);
            }
            catch (Exception ex)
            {
                LogException("WaitForFirstElement", ex);
                throw;
            }
        }

        public static bool WaitForTextToBePresentInElement(this IApp app, string automationId, string text, TimeSpan? timeout = null)
        {

            try
            {
                timeout ??= DefaultTimeout;
                TimeSpan retryFrequency = TimeSpan.FromMilliseconds(500);

                DateTime start = DateTime.Now;

                while (true)
                {
                    var element = app.FindElements(automationId).FirstOrDefault();
                    if (element != null && (element.GetText()?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false))
                    {
                        return true;
                    }

                    long elapsed = DateTime.Now.Subtract(start).Ticks;
                    if (elapsed >= timeout.Value.Ticks)
                    {
                        Debug.WriteLine($">>>>> {elapsed} ticks elapsed, timeout value is {timeout.Value.Ticks}");

                        return false;
                    }

                    Task.Delay(retryFrequency.Milliseconds).Wait();
                }
            }
            catch (Exception ex)
            {
                LogException("WaitForTextToBePresentInElement", ex);
                throw;
            }
        }

        /// <summary>
        /// Repeatedly executes a query until it returns a non-empty value or the specified retry count is reached.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="func">The query to execute.</param>
        /// <param name="retryCount">The number of times to retry execution. Default is 10.</param>
        /// <param name="delayInMs">The delay in milliseconds between retries. Default is 2000ms.</param>
        /// <returns>An value of type T.</returns>
        public static T QueryUntilPresent<T>(
            this IApp app,
            Func<T> func,
            int retryCount = 10,
            int delayInMs = 2000)
        {
            var result = func();

            int counter = 0;
            while ((result is null) && counter < retryCount)
            {
                Thread.Sleep(delayInMs);
                result = func();
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Repeatedly executes a query until it returns a null value or the specified retry count is reached.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="func">The query to execute.</param>
        /// <param name="retryCount">The number of times to retry execution. Default is 10.</param>
        /// <param name="delayInMs">The delay in milliseconds between retries. Default is 2000ms.</param>
        /// <returns>An value of type T.</returns>
        public static T QueryUntilNotPresent<T>(
            this IApp app,
            Func<T> func,
            int retryCount = 10,
            int delayInMs = 2000)
        {
            var result = func();

            int counter = 0;
            while ((result is not null) && counter < retryCount)
            {
                Thread.Sleep(delayInMs);
                result = func();
                counter++;
            }

            return result;
        }

        /// <summary>
        /// Presses the volume up button on the device.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void PressVolumeUp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("pressVolumeUp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("PressVolumeUp", ex);
                throw;
            }
        }

        /// <summary>
        /// Presses the volume down button on the device.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void PressVolumeDown(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("pressVolumeDown", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("PressVolumeDown", ex);
                throw;
            }
        }

        /// <summary>
        /// Presses the enter key in the app.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void PressEnter(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("pressEnter", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("PressEnter", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a left to right swipe gesture on the screen. 
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeLeftToRight(this IApp app, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                app.CommandExecutor.Execute("swipeLeftToRight", new Dictionary<string, object>
            {
                { "swipePercentage", swipePercentage },
                { "swipeSpeed", swipeSpeed },
                { "withInertia", withInertia }
            });
            }
            catch (Exception ex)
            {
                LogException("SwipeLeftToRight", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a left to right swipe gesture on the matching element. 
        /// If multiple elements are matched, the first one will be used.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeLeftToRight(this IApp app, string marked, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToSwipe = FindElement(app, marked);
                app.SwipeLeftToRight(elementToSwipe, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("SwipeLeftToRight", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a left to right swipe gesture on an element matched by 'query'.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeLeftToRight(this IApp app, IQuery query, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToSwipe = app.FindElement(query);

                app.SwipeLeftToRight(elementToSwipe, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("SwipeLeftToRight", ex);
                throw;
            }
        }

        internal static void SwipeLeftToRight(this IApp app, IUIElement? element, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("swipeLeftToRight", new Dictionary<string, object>
                {
                    { "element", element },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("SwipeLeftToRight", ex);
                throw;
            }
        }

        /// <summary>
        ///  Performs a right to left swipe gesture on the screen. 
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeRightToLeft(this IApp app, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                app.CommandExecutor.Execute("swipeRightToLeft", new Dictionary<string, object>
            {
                { "swipePercentage", swipePercentage },
                { "swipeSpeed", swipeSpeed },
                { "withInertia", withInertia }
            });
            }
            catch (Exception ex)
            {
                LogException("SwipeRightToLeft", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a right to left swipe gesture on the matching element. 
        /// If multiple elements are matched, the first one will be used.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeRightToLeft(this IApp app, string marked, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToSwipe = FindElement(app, marked);

                app.SwipeRightToLeft(elementToSwipe, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("SwipeRightToLeft", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a right to left swipe gesture on an element matched by 'query'.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void SwipeRightToLeft(this IApp app, IQuery query, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToSwipe = app.FindElement(query);
                app.SwipeRightToLeft(elementToSwipe, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("SwipeRightToLeft", ex);
                throw;
            }
        }

        internal static void SwipeRightToLeft(this IApp app, IUIElement? element, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {

            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("swipeRightToLeft", new Dictionary<string, object>
                {
                    { "element", element },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("SwipeRightToLeft", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls left on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollLeft(this IApp app, string marked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = FindElement(app, marked);

                app.ScrollLeft(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollLeft", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls left on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollLeft(this IApp app, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);
                app.ScrollLeft(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollLeft", ex);
                throw;
            }
        }

        internal static void ScrollLeft(this IApp app, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {

            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("scrollLeft", new Dictionary<string, object>
                {
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("ScrollLeft", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls down on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollDown(this IApp app, string marked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = FindElement(app, marked);
                app.ScrollDown(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollDown", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls down on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollDown(this IApp app, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);
                app.ScrollDown(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollDown", ex);
                throw;
            }
        }

        internal static void ScrollDown(this IApp app, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {

            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("scrollDown", new Dictionary<string, object>
                {
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("ScrollDown", ex);
                throw;
            }
        }

        /// <summary>
        /// Scroll down until an element that matches the toMarked is shown on the screen.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="toMarked">Marked selector to select what element to bring on screen.</param>
        /// <param name="withinMarked">Marked selector to select what element to scroll within.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollDownTo(this IApp app, string toMarked, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = FindElement(app, withinMarked);

                app.ScrollDownTo(toMarked, elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollDown", ex);
                throw;
            }
        }

        /// <summary>
        /// Scroll down until an element that matches the query is shown on the screen.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="toMarked">Marked selector to select what element to bring on screen.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollDownTo(this IApp app, string toMarked, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);

                app.ScrollDownTo(toMarked, elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollDownTo", ex);
                throw;
            }
        }

        internal static void ScrollDownTo(this IApp app, string toMarked, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            if (element is not null)
            {
                app.CommandExecutor.Execute("scrollDown", new Dictionary<string, object>
                {
                    { "marked", toMarked },
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
            }
        }

        /// <summary>
        /// Scrolls right on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollRight(this IApp app, string marked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToSwipe = FindElement(app, marked);

                app.ScrollRight(elementToSwipe, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollRight", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls right on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollRight(this IApp app, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);
                app.ScrollRight(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollRight", ex);
                throw;
            }
        }

        internal static void ScrollRight(this IApp app, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("scrollRight", new Dictionary<string, object>
                {
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("ScrollRight", ex);
                throw;
            }
        }

        /// <summary>
        /// Scrolls up on the first element matching query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector to match.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollUp(this IApp app, string marked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = FindElement(app, marked);
                app.ScrollUp(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollUp", ex);
                throw;
            }
        }

        public static void ScrollUp(this IApp app, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);
                app.ScrollUp(elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollUp", ex);
                throw;
            }
        }

        public static void ScrollUp(this IApp app, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("scrollUp", new Dictionary<string, object>
                {
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("ScrollUp", ex);
                throw;
            }
        }

        /// <summary>
        /// Scroll up until an element that matches the <paramref name="toMarked"/> is shown on the screen in <paramref name="withinMarked"/>.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="toMarked">Marked selector to select what element to bring on screen.</param>
        /// <param name="withinMarked">Marked selector to select what element to scroll within.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollUpTo(this IApp app, string toMarked, string withinMarked, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = FindElement(app, withinMarked);
                app.ScrollUpTo(toMarked, elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollUpTo", ex);
                throw;
            }
        }

        /// <summary>
        /// Scroll up until an element that matches <paramref name="toMarked"/> is shown on the screen.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="toMarked">Marked selector to select what element to bring on screen.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="strategy">Strategy for scrolling element.</param>
        /// <param name="swipePercentage">How far across the element to swipe (from 0.0 to 1.0).</param>
        /// <param name="swipeSpeed">The speed of the gesture.</param>
        /// <param name="withInertia">Whether swipes should cause inertia.</param>
        public static void ScrollUpTo(this IApp app, string toMarked, IQuery query, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {
            try
            {
                var elementToScroll = app.FindElement(query);

                app.ScrollUpTo(toMarked, elementToScroll, strategy, swipePercentage, swipeSpeed, withInertia);
            }
            catch (Exception ex)
            {
                LogException("ScrollUpTo", ex);
                throw;
            }
        }

        internal static void ScrollUpTo(this IApp app, string toMarked, IUIElement? element, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true)
        {

            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("scrollUpTo", new Dictionary<string, object>
                {
                    { "marked", toMarked },
                    { "element", element },
                    { "strategy", strategy },
                    { "swipePercentage", swipePercentage },
                    { "swipeSpeed", swipeSpeed },
                    { "withInertia", withInertia }
                });
                }
            }
            catch (Exception ex)
            {
                LogException("ScrollUpTo", ex);
                throw;
            }
        }

        /// <summary>
        /// Changes the device orientation to landscape mode.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void SetOrientationLandscape(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("setOrientationLandscape", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("SetOrientationLandscape", ex);
                throw;
            }
        }

        /// <summary>
        /// Changes the device orientation to portrait mode.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void SetOrientationPortrait(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("setOrientationPortrait", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("SetOrientationPortrait", ex);
                throw;
            }
        }

        /// <summary>
        /// Get the current device orientation.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <returns>The current device orientation</returns>
        public static OpenQA.Selenium.ScreenOrientation GetOrientation(this IApp app)
        {
            try
            {
                var response = app.CommandExecutor.Execute("getOrientation", new Dictionary<string, object>());

                if (response?.Value != null)
                {
                    return (OpenQA.Selenium.ScreenOrientation)response.Value;
                }

                throw new InvalidOperationException($"Could not get the current orientation");
            }
            catch (Exception ex)
            {
                LogException("GetOrientation", ex);
                throw;
            }
        }

        /// <summary>
        /// Get the text of the system clipboard.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <returns>Clipboard content as string or an empty string if the clipboard is empty.</returns>
        public static string GetClipboardText(this IApp app)
        {
            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumIOSApp)
                {
                    throw new InvalidOperationException($"GetClipboard is not supported");
                }

                var response = app.CommandExecutor.Execute("getClipboardText", new Dictionary<string, object>());

                if (response?.Value != null)
                {
                    return (string)response.Value;
                }

                throw new InvalidOperationException($"Could not get clipboard text");
            }
            catch (Exception ex)
            {
                LogException("GetClipboardText", ex);
                throw;
            }
        }

        /// <summary>
        /// Set the content of the system clipboard.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="content">The actual clipboard content.</param>
        /// <param name="label">Clipboard data label for Android.</param>
        public static void SetClipboardText(this IApp app, string content, string? label = null)
        {
            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumIOSApp)
                {
                    throw new InvalidOperationException($"SetClipboard is not supported");
                }

                app.CommandExecutor.Execute("setClipboardText", new Dictionary<string, object>
            {
                { "content", content },
                { "label", label! }
            });
            }
            catch (Exception ex)
            {
                LogException("SetClipboardText", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a mouse click on the given coordinates.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate to click.</param>
        /// <param name="y">The y coordinate to click.</param>
        public static void ClickCoordinates(this IApp app, float x, float y)
        {
            try
            {
                app.CommandExecutor.Execute("clickCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y }
            });
            }
            catch (Exception ex)
            {
                LogException("ClickCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a tap / touch gesture on the given coordinates.
        /// This API currently doesn't work on Catalyst https://github.com/dotnet/maui/issues/19754
        /// For Catalyst you'll currently need to use Click instead. 
        /// Tap is more mobile-specific and provides more flexibility than click. Click is more general and is 
        /// used for simpler interactions. Depending on the context of your test, you might prefer one over the other.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="x">The x coordinate to tap.</param>
        /// <param name="y">The y coordinate to tap.</param>
        public static void TapCoordinates(this IApp app, float x, float y)
        {
            try
            {
                app.CommandExecutor.Execute("tapCoordinates", new Dictionary<string, object>
            {
                { "x", x },
                { "y", y }
            });
            }
            catch (Exception ex)
            {
                LogException("TapCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Executes an existing application on the device. 
        /// If the application is already running then it will be brought to the foreground.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void LaunchApp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("launchApp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("LaunchApp", ex);
                throw;
            }
        }

        /// <summary>
        /// Send the currently running app for this session to the background.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void BackgroundApp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("backgroundApp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("BackgroundApp", ex);
                throw;
            }
        }

        /// <summary>
        /// If the application is already running then it will be brought to the foreground.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void ForegroundApp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("foregroundApp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("ForegroundApp", ex);
                throw;
            }
        }

        /// <summary>
        /// Reset the currently running app for this session.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void ResetApp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("resetApp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("ResetApp", ex);
                throw;
            }
        }

        /// <summary>
        /// Close an app on device.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void CloseApp(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("closeApp", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("CloseApp", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the value of a Slider element that matches marked.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector of the Slider element to update.</param>
        /// <param name="value">The value to set the Slider to.</param>
        public static void SetSliderValue(this IApp app, string marked, double value)
        {
            try
            {
                var element = FindElement(app, marked);

                double defaultMinimum = 0d;
                double defaultMaximum = 1d;

                app.CommandExecutor.Execute("setSliderValue", new Dictionary<string, object>
            {
                { "element", element },
                { "value", value },
                { "minimum", defaultMinimum },
                { "maximum", defaultMaximum },
            });
            }
            catch (Exception ex)
            {
                LogException("SetSliderValue", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the value of a Slider element that matches marked.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector of the Slider element to update.</param>
        /// <param name="value">The value to set the Slider to.</param>
        /// <param name="minimum">Te minimum selectable value for the Slider.</param>
        /// <param name="maximum">Te maximum selectable value for the Slider.</param>
        public static void SetSliderValue(this IApp app, string marked, double value, double minimum = 0d, double maximum = 1d)
        {
            try
            {
                var element = FindElement(app, marked);

                app.SetSliderValue(element, value, minimum, maximum);
            }
            catch (Exception ex)
            {
                LogException("SetSliderValue", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the value of a slider element that matches query.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="query">Represents the query that identify an element by parameters such as type, text it contains or identifier.</param>
        /// <param name="value">The value to set the Slider to.</param>
        /// <param name="minimum">Te minimum selectable value for the Slider.</param>
        /// <param name="maximum">Te maximum selectable value for the Slider.</param>
        public static void SetSliderValue(this IApp app, IQuery query, double value, double minimum = 0d, double maximum = 1d)
        {
            try
            {
                var element = app.FindElement(query);

                app.SetSliderValue(element, value, minimum, maximum);
            }
            catch (Exception ex)
            {
                LogException("SetSliderValue", ex);
                throw;
            }
        }

        internal static void SetSliderValue(this IApp app, IUIElement? element, double value, double minimum = 0d, double maximum = 1d)
        {

            try
            {
                if (element is not null)
                {
                    app.CommandExecutor.Execute("setSliderValue", new Dictionary<string, object>
                {
                    { "element", element },
                    { "value", value },
                    { "minimum", minimum },
                    { "maximum", maximum },
                });
                }
            }
            catch (Exception ex)
            {
                LogException("SetSliderValue", ex);
                throw;
            }
        }

        /// <summary>
        /// Increases the value of a Stepper control.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector of the Stepper element to increase.</param>
        public static void IncreaseStepper(this IApp app, string marked)
        {
            try
            {
                app.CommandExecutor.Execute("increaseStepper", new Dictionary<string, object>
                {
                    ["elementId"] = marked
                });
            }
            catch (Exception ex)
            {
                LogException("IncreaseStepper", ex);
                throw;
            }
        }

        /// <summary>
        /// Decreases the value of a Stepper control.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="marked">Marked selector of the Stepper element to decrease.</param>
        public static void DecreaseStepper(this IApp app, string marked)
        {
            try
            {
                app.CommandExecutor.Execute("decreaseStepper", new Dictionary<string, object>
                {
                    ["elementId"] = marked
                });
            }
            catch (Exception ex)
            {
                LogException("DecreaseStepper", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a continuous drag gesture between 2 points.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="fromX">The x coordinate to start dragging from.</param>
        /// <param name="fromY">The y coordinate to start dragging from.</param>
        /// <param name="toX">The x coordinate to drag to.</param>
        /// <param name="toY">The y coordinate to drag to.</param>
        public static void DragCoordinates(this IApp app, float fromX, float fromY, float toX, float toY)
        {

            try
            {
                app.CommandExecutor.Execute("dragCoordinates", new Dictionary<string, object>
            {
                { "fromX", fromX },
                { "fromY", fromY },
                { "toX", toX },
                { "toY", toY },
            });
            }
            catch (Exception ex)
            {
                LogException("DragCoordinates", ex);
                throw;
            }
        }

        /// <summary>
        /// Performs a pan gesture between 2 points.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="fromX">The x coordinate to start panning from.</param>
        /// <param name="fromY">The y coordinate to start panning from.</param>
        /// <param name="toX">The x coordinate to pan to.</param>
        /// <param name="toY">The y coordinate to pan to.</param>
        public static void Pan(this IApp app, float fromX, float fromY, float toX, float toY)
        {
            try
            {
                app.DragCoordinates(fromX, fromY, toX, toY);
            }
            catch (Exception ex)
            {
                LogException("Pan", ex);
                throw;
            }
        }

        /// <summary>
        /// Navigate back on the device.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void Back(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("back", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("Back", ex);
                throw;
            }
        }

        /// <summary>
        /// Refresh the current page.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void Refresh(this IApp app)
        {
            try
            {
                app.CommandExecutor.Execute("refresh", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("Refresh", ex);
                throw;
            }
        }

        /// <summary>
        /// Return the AppId of the running app. This is used inside any appium command that want the app id
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static string GetAppId(this IApp app)
        {
            try
            {
                if (app is not AppiumApp aaa)
                {
                    throw new InvalidOperationException($"GetAppId is only supported on AppiumApp");
                }

                var appId = aaa.Config.GetProperty<string>("AppId");
                if (appId is not null)
                {
                    return appId;
                }

                throw new InvalidOperationException("AppId not found");
            }
            catch (Exception ex)
            {
                LogException("GetAppId", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve the target device this test is running against
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TestDevice GetTestDevice(this IApp app)
        {
            try
            {
                if (app is not AppiumApp aaa)
                {
                    throw new InvalidOperationException($"GetTestDevice is only supported on AppiumApp");
                }

                return aaa.Config.GetProperty<TestDevice>("TestDevice");
            }
            catch (Exception ex)
            {
                LogException("GetTestDevice", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets light device's theme
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void SetLightTheme(this IApp app)
        {
            try
            {
                if (app is AppiumCatalystApp)
                {
                    throw new InvalidOperationException($"SetLightTheme is not supported");
                }

                app.CommandExecutor.Execute("setLightTheme", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("SetLightTheme", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets dark device's theme
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void SetDarkTheme(this IApp app)
        {
            try
            {
                if (app is AppiumCatalystApp)
                {
                    throw new InvalidOperationException($"SetDarkTheme is not supported");
                }

                app.CommandExecutor.Execute("setDarkTheme", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("SetDarkTheme", ex);
                throw;
            }
        }

        /// <summary>
        /// Check if element has focused
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="id">Target element</param>
        /// <returns>Returns <see langword="true"/> if focused</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsFocused(this IApp app, string id)
        {
            try
            {
                if (app is not AppiumApp aaa)
                {
                    throw new InvalidOperationException($"IsFocused is only supported on AppiumApp");
                }

                var activeElement = aaa.Driver.SwitchTo().ActiveElement();
                var element = (AppiumDriverElement)app.WaitForElement(id);

                if (app.GetTestDevice() == TestDevice.Mac && activeElement is AppiumElement activeAppiumElement)
                {
                    // For some reason on catalyst the ActiveElement returns an AppiumElement with a different id
                    // The TagName (AutomationId) and the location all match, so, other than the Id it walks and talks
                    // like the same element
                    return element.AppiumElement.TagName.Equals(activeAppiumElement.TagName, StringComparison.OrdinalIgnoreCase) &&
                        element.AppiumElement.Location.Equals(activeAppiumElement.Location);
                }

                return element.AppiumElement.Equals(activeElement);
            }
            catch (Exception ex)
            {
                LogException("IsFocused", ex);
                throw;
            }
        }

        /// <summary>
        /// Lock the screen.
        /// Functionality that's only available on Android and iOS.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">Lock is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void Lock(this IApp app)
        {
            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumIOSApp)
                {
                    throw new InvalidOperationException($"Lock is only supported on AppiumAndroidApp and AppiumIOSApp");
                }

                app.CommandExecutor.Execute("lock", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("Lock", ex);
                throw;
            }
        }

        /// <summary>
        /// Unlock the screen.
        /// Functionality that's only available on Android and iOS.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="unlockType">This capability supports the following possible values: pin, pinWithKeyEvent, password, pattern.</param>
        /// <param name="unlockKey">a valid pin (digits in range 0-9), password (latin characters) or pattern (treat the pattern pins similarly to numbers on a digital phone dial).</param>
        /// <exception cref="InvalidOperationException">Unlock is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void Unlock(this IApp app, string unlockType = "", string unlockKey = "")
        {
            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumIOSApp)
                {
                    throw new InvalidOperationException($"Unlock is only supported on AppiumAndroidApp and AppiumIOSApp");
                }

                app.CommandExecutor.Execute("unlock", new Dictionary<string, object>()
            {
                { "unlockType", unlockType },
                { "unlockKey", unlockKey },
            });
            }
            catch (Exception ex)
            {
                LogException("Unlock", ex);
                throw;
            }
        }

        /// <summary>
        /// Check whether the device is locked or not.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static bool IsLocked(this IApp app)
        {
            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumIOSApp)
                {
                    throw new InvalidOperationException($"IsLocked is only supported on AppiumAndroidApp and AppiumIOSApp");
                }
                var response = app.CommandExecutor.Execute("isLocked", new Dictionary<string, object>());

                var responseValue = response?.Value ?? false;

                return (bool)responseValue;
            }
            catch (Exception ex)
            {
                LogException("IsLocked", ex);
                throw;
            }
        }

        /// <summary>
        /// Perform a shake action on the device.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void Shake(this IApp app)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"Shake is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("shake", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("Shake", ex);
                throw;
            }
        }

        /// <summary>
        /// Start recording screen.
        /// Functionality that's only available on Android, iOS and Windows.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">StartRecordingScreen is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void StartRecordingScreen(this IApp app)
        {
            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"StartRecordingScreen is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("startRecordingScreen", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("StartRecordingScreen", ex);
                throw;
            }
        }

        /// <summary>
        /// Stop recording screen.
        /// Functionality that's only available on Android, iOS and Windows.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">StopRecordingScreen is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void StopRecordingScreen(this IApp app)
        {
            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"StopRecordingScreen is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("stopRecordingScreen", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("StopRecordingScreen", ex);
                throw;
            }
        }

        /// <summary>
        /// Toggle airplane mode on device.
        /// Functionality that's only available on Android.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">ToggleAirplaneMode is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void ToggleAirplaneMode(this IApp app)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"ToggleAirplaneMode is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("toggleAirplaneMode", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("ToggleAirplaneMode", ex);
                throw;
            }
        }

        /// <summary>
        /// Switch the state of the wifi service.
        /// Functionality that's only available on Android. 
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">ToggleWifi is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void ToggleWifi(this IApp app)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"ToggleWifi is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("toggleWifi", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("TapToggleWifi", ex);
                throw;
            }
        }

        /// <summary>
        /// Switch the System animations state.
        /// Optimize and accelerate tests, eliminating animations entirely when Appium is executing tests, as they serve no practical purpose in this context.
        /// Functionality that's only available on Android and Catalyst.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="enableSystemAnimations">Enable/disable the system animations.</param>
        /// <exception cref="InvalidOperationException">ToggleSystemAnimations is only supported on <see cref="AppiumAndroidApp"/> and <see cref="AppiumCatalystApp"/>.</exception>
        public static void ToggleSystemAnimations(this IApp app, bool enableSystemAnimations)
        {

            try
            {
                if (app is not AppiumAndroidApp && app is not AppiumCatalystApp)
                {
                    throw new InvalidOperationException($"ToggleSystemAnimations is not supported");
                }

                app.CommandExecutor.Execute("toggleSystemAnimations", new Dictionary<string, object>()
            {
                { "enableSystemAnimations", enableSystemAnimations },
            });
            }
            catch (Exception ex)
            {
                LogException("ToggleSystemAnimations", ex);
                throw;
            }
        }

        /// <summary>
        /// Switch the state of data service.
        /// Functionality that's only available on Android.
        /// This API does not work for Android API level 21+ because it requires system or carrier privileged permission, 
        /// and Android <= 21 does not support granting permissions.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <exception cref="InvalidOperationException">ToggleData is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        public static void ToggleData(this IApp app)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"ToggleData is only supported on AppiumAndroidApp");
                }

                app.CommandExecutor.Execute("toggleData", ImmutableDictionary<string, object>.Empty);
            }
            catch (Exception ex)
            {
                LogException("ToggleData", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the information of the system state which is supported to read as like cpu, memory, network traffic, and battery.
        /// Functionality that's only available on Android.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="performanceDataType">The available performance data types(cpuinfo | batteryinfo | networkinfo | memoryinfo).</param>
        /// <exception cref="InvalidOperationException">ToggleWifi is only supported on <see cref="AppiumAndroidApp"/>.</exception>
        /// <returns>The information of the system related to the performance.</returns>
        public static IList<object> GetPerformanceData(this IApp app, string performanceDataType)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"GetPerformanceData is only supported on AppiumAndroidApp");
                }

                var response = app.CommandExecutor.Execute("getPerformanceData", new Dictionary<string, object>()
            {
                { "performanceDataType", performanceDataType },
            });

                if (response?.Value != null)
                {
                    return (IList<object>)response.Value;
                }

                throw new InvalidOperationException($"Could not get the performance data");
            }
            catch (Exception ex)
            {
                LogException("GetPerformanceData", ex);
                throw;
            }
        }

        /// <summary>
        /// Maximize the active App window.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void EnterFullScreen(this IApp app)
        {
            try
            {
                if (app is not AppiumCatalystApp)
                {
                    throw new InvalidOperationException($"EnterFullScreen is only supported on AppiumCatalystApp");
                }

                app.CommandExecutor.Execute("enterFullScreen", new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogException("EnterFullScreen", ex);
                throw;
            }
        }

        /// <summary>
        /// Leave the App full screen mode.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void ExitFullScreen(this IApp app)
        {
            try
            {
                if (app is not AppiumCatalystApp)
                {
                    throw new InvalidOperationException($"ExitFullScreen is only supported on AppiumCatalystApp");
                }

                app.CommandExecutor.Execute("exitFullScreen", new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                LogException("ExitFullScreen", ex);
                throw;
            }
        }

        /// <summary>
        /// Print in the Output the current application hierarchy XML (app).
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void PrintTree(this IApp app)
        {

            try
            {
                if (app is not AppiumApp aaa)
                {
                    throw new InvalidOperationException($"PrintTree is only supported on AppiumApp");
                }

                var pageSource = aaa.Driver.PageSource;
                Console.WriteLine(pageSource);
            }
            catch (Exception ex)
            {
                LogException("PrintTree", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve visibility and bounds information of the status and navigation bars
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <returns>Information about visibility and bounds of status and navigation bar.</returns>
        public static IDictionary<string, object> GetSystemBars(this IApp app)
        {

            try
            {
                if (app is not AppiumAndroidApp)
                {
                    throw new InvalidOperationException($"GetSystemBars is only supported on AppiumAndroidApp");
                }

                var response = app.CommandExecutor.Execute("getSystemBars", new Dictionary<string, object>());

                if (response?.Value != null)
                {
                    return (IDictionary<string, object>)response.Value;
                }

                throw new InvalidOperationException($"Could not get the Android System Bars");
            }
            catch (Exception ex)
            {
                LogException("GetSystemBars", ex);
                throw;
            }
        }

        /// <summary>
        /// Navigates back in the application by simulating a tap on the platform-specific back navigation button or using a custom identifier.
        /// </summary>
        /// <param name="app">The IApp instance representing the main gateway to interact with the application.</param>
        /// <param name="customBackButtonIdentifier">Optional custom identifier string for the back button. If not provided, the default back arrow query will be used.</param>
        public static void TapBackArrow(this IApp app, string customBackButtonIdentifier = "")
        {

            try
            {
                var query = string.IsNullOrEmpty(customBackButtonIdentifier)
                ? GetDefaultBackArrowQuery(app)
                : GetCustomBackArrowQuery(app, customBackButtonIdentifier);

                TapBackArrow(app, query);
            }
            catch (Exception ex)
            {
                LogException("TapBackArrow", ex);
                throw;
            }
        }

        /// <summary>
        /// Navigates back in the application using a custom IQuery.
        /// </summary>
        /// <param name="app">The IApp instance representing the main gateway to interact with the application.</param>
        /// <param name="query">The custom IQuery for the back button.</param>
        public static void TapBackArrow(this IApp app, IQuery query)
        {

            try
            {
                app.WaitForElement(query).Tap();
            }
            catch (Exception ex)
            {
                LogException("TapBackArrow", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps a button in a display alert dialog.
        /// For AppiumCatalystApp, it uses specific element identifiers to locate and tap the alert button.
        /// For other app types, it locates and taps the button using the provided text.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="text">The text of the button to tap in the display alert (used for non-AppiumCatalystApp instances).</param>
        /// <param name="buttonIndex">
        /// The index of the button in the alert dialog, used to generate the correct element identifier.
        /// For example, in a alert with two buttons:
        /// - 0 (default) corresponds to the leftmost button (e.g., "OK" with identifier ending in 999)
        /// - 1 corresponds to the button to its right (e.g., "Cancel" with identifier ending in 998)
        /// </param>
        public static void TapDisplayAlertButton(this IApp app, string text, int buttonIndex = 0)
        {

            try
            {
                if (app is AppiumCatalystApp)
                {
                    app.WaitForElement(AppiumQuery.ById($"action-button--{999 - buttonIndex}"));
                    app.Tap(AppiumQuery.ById($"action-button--{999 - buttonIndex}"));
                }
                else
                {
                    app.WaitForElement(text);
                    app.Tap(text);
                }
            }
            catch (Exception ex)
            {
                LogException("TapDisplayAlertButton", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the default query for the back arrow button based on the app type.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <returns>An IQuery for the default back arrow button.</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported app type is provided.</exception>
        static IQuery GetDefaultBackArrowQuery(IApp app)
        {
            try
            {
                return app switch
                {
                    AppiumAndroidApp _ => AppiumQuery.ByXPath("//android.widget.ImageButton[@content-desc='Navigate up']"),
                    AppiumIOSApp _ => AppiumQuery.ByAccessibilityId("Back"),
                    AppiumCatalystApp _ => AppiumQuery.ByAccessibilityId("Back"),
                    AppiumWindowsApp _ => AppiumQuery.ByAccessibilityId("NavigationViewBackButton"),
                    _ => throw new ArgumentException("Unsupported app type", nameof(app))
                };
            }
            catch (Exception ex)
            {
                LogException("GetDefaultBackArrowQuery", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a custom query for the back arrow button based on the app type and a custom identifier.
        /// Note that for Windows apps, the back button is not customizable, so the default identifier is used.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="customBackButtonIdentifier">The custom identifier for the back button.</param>
        /// <returns>An IQuery for the custom back arrow button.</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported app type is provided.</exception>
        static IQuery GetCustomBackArrowQuery(IApp app, string customBackButtonIdentifier)
        {
            try
            {
                return app switch
                {
                    AppiumAndroidApp _ => AppiumQuery.ByXPath($"//android.widget.ImageButton[@content-desc='{customBackButtonIdentifier}']"),
                    AppiumIOSApp _ => AppiumQuery.ByXPath($"//XCUIElementTypeButton[@name='{customBackButtonIdentifier}']"),
                    AppiumCatalystApp _ => AppiumQuery.ByName(customBackButtonIdentifier),
                    AppiumWindowsApp _ => AppiumQuery.ByAccessibilityId("NavigationViewBackButton"),
                    _ => throw new ArgumentException("Unsupported app type", nameof(app))
                };
            }
            catch (Exception ex)
            {
                LogException("GetCustomBackArrowQuery", ex);
                throw;
            }
        }

        /// <summary>
        /// Waits for an element to be ready until page navigation has settled, with additional waiting for MacCatalyst.
        /// This method helps prevent null reference exceptions during page transitions, especially in MacCatalyst.
        /// </summary>
        /// <param name="app">The IApp instance.</param>
        /// <param name="elementId">The id of the element to wait for.</param>
        /// <param name="timeout">Optional timeout for the wait operation. Default is null, which uses the default timeout.</param>
        public static IUIElement WaitForElementTillPageNavigationSettled(this IApp app, string elementId, TimeSpan? timeout = null)
        {
            try
            {
                if (app is AppiumCatalystApp)
                    app.WaitForElement(AppiumQuery.ById(elementId), timeout: timeout);

                return app.WaitForElement(elementId, timeout: timeout);
            }
            catch (Exception ex)
            {
                LogException("WaitForElementTillPageNavigationSettled", ex);
                throw;
            }
        }

        /// <summary>
        /// Waits for an element to be ready until page navigation has settled, with additional waiting for MacCatalyst.
        /// This method helps prevent null reference exceptions during page transitions, especially in MacCatalyst.
        /// </summary>
        /// <param name="app">The IApp instance.</param>
        /// <param name="query">The query to use for finding the element.</param>
        /// <param name="timeout">Optional timeout for the wait operation. Default is null, which uses the default timeout.</param>
        public static void WaitForElementTillPageNavigationSettled(this IApp app, IQuery query, TimeSpan? timeout = null)
        {

            try
            {
                if (app is AppiumCatalystApp)
                    app.WaitForElement(query, timeout: timeout);

                app.WaitForElement(query, timeout: timeout);
            }
            catch (Exception ex)
            {
                LogException("WaitForElementTillPageNavigationSettled", ex);
                throw;
            }
        }

        /// <summary>
        /// Waits for the flyout icon to appear in the app.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="automationId">The automation ID of the flyout icon (default is an empty string).</param>
        /// <param name="isShell">Indicates whether the app is using Shell navigation (default is true).</param>
        public static void WaitForFlyoutIcon(this IApp app, string automationId = "", bool isShell = true)
        {
            try
            {
                if (app is AppiumAndroidApp)
                {
                    app.WaitForElement(AppiumQuery.ByXPath("//android.widget.ImageButton[@content-desc=\"Open navigation drawer\"]"));
                }
                else if (app is AppiumIOSApp || app is AppiumCatalystApp || app is AppiumWindowsApp)
                {
                    if (isShell)
                    {
                        app.WaitForElement("OK");
                    }
                    if (!isShell)
                    {
                        if (app is AppiumWindowsApp)
                        {
                            app.WaitForElement(AppiumQuery.ByAccessibilityId("TogglePaneButton"));
                        }
                        else
                        {
                            app.WaitForElement(automationId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("WaitForFlyoutIcon", ex);
                throw;
            }
        }

        /// <summary>
        /// Shows the flyout menu in the app.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="automationId">The automation ID of the flyout icon (default is an empty string).</param>
        /// <param name="usingSwipe">Indicates whether to use swipe gesture to open the flyout (default is false).</param>
        /// <param name="waitForFlyoutIcon">Indicates whether to wait for the flyout icon before showing the flyout (default is true).</param>
        /// <param name="isShell">Indicates whether the app is using Shell navigation (default is true).</param>
        public static void ShowFlyout(this IApp app, string automationId = "", bool usingSwipe = false, bool waitForFlyoutIcon = true, bool isShell = true)
        {
            try
            {
                if (waitForFlyoutIcon)
                {
                    app.WaitForFlyoutIcon(automationId, isShell);
                }

                if (usingSwipe)
                {
                    app.DragCoordinates(5, 500, 800, 500);
                }
                else
                {
                    app.TapFlyoutIcon(automationId, isShell, false);
                }
            }
            catch (Exception ex)
            {
                LogException("ShowFlyout", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps the Flyout icon for Shell or FlyoutPage.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="title">Optional title for FlyoutPage (default is empty string).</param>
        /// <param name="isShell">Indicates whether the Flyout is for Shell (true) or FlyoutPage (false).</param>
        private static void TapFlyoutIcon(this IApp app, string title = "", bool isShell = true, bool waitForFlyoutIcon = true)
        {
            try
            {
                if (waitForFlyoutIcon)
                {
                    app.WaitForFlyoutIcon(title, isShell);
                }
                if (app is AppiumAndroidApp)
                {
                    app.Tap(AppiumQuery.ByXPath("//android.widget.ImageButton[@content-desc=\"Open navigation drawer\"]"));
                }
                else if (app is AppiumIOSApp || app is AppiumCatalystApp || app is AppiumWindowsApp)
                {
                    if (isShell)
                    {
                        app.Tap(AppiumQuery.ByAccessibilityId("OK"));
                    }
                    else
                    {
                        if (app is AppiumWindowsApp)
                        {
                            app.Tap(AppiumQuery.ByAccessibilityId("TogglePaneButton"));
                        }
                        else
                        {
                            app.Tap(title);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("TapFlyoutIcon", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps the Flyout icon for Shell pages.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void TapShellFlyoutIcon(this IApp app)
        {
            try
            {
                app.TapFlyoutIcon();
            }
            catch (Exception ex)
            {
                LogException("TapShellFlyoutIcon", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps the Flyout icon for FlyoutPage.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        /// <param name="title">Optional title for FlyoutPage (default is empty string).</param>
        public static void TapFlyoutPageIcon(this IApp app, string title = "")
        {
            try
            {
                app.TapFlyoutIcon(title, false);
            }
            catch (Exception ex)
            {
                LogException("TapFlyoutPageIcon", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps an item in the specified flyout menu.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="flyoutItem">The text or accessibility identifier of the flyout item to tap.</param>
        /// <param name="isShellFlyout">True if it's a Shell flyout, false for FlyoutPage flyout.</param>
        private static void TapInFlyout(this IApp app, string flyoutItem, bool isShellFlyout)
        {
            try
            {
                if (isShellFlyout)
                {
                    app.TapShellFlyoutIcon();
                }
                else
                {
                    app.TapFlyoutPageIcon();
                }

                app.WaitForElement(flyoutItem);
                app.Tap(flyoutItem);
            }
            catch (Exception ex)
            {
                LogException("TapInFlyout", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps an item in the Shell flyout menu.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="flyoutItem">The text or accessibility identifier of the flyout item to tap.</param>
        public static void TapInShellFlyout(this IApp app, string flyoutItem)
        {
            try
            {
                app.TapInFlyout(flyoutItem, true);
            }
            catch (Exception ex)
            {
                LogException("TapInShellFlyout", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps an item in the FlyoutPage flyout menu.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="flyoutItem">The text or accessibility identifier of the flyout item to tap.</param>
        public static void TapInFlyoutPageFlyout(this IApp app, string flyoutItem)
        {
            try
            {
                app.TapInFlyout(flyoutItem, false);
            }
            catch (Exception ex)
            {
                LogException("TapInFlyoutPageFlyout", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps the "More" button in the app, with platform-specific logic for Android and Windows.
        /// This method does not currently support iOS and macOS platforms, where the "More" button is not shown.
        /// </summary>
        /// <param name="app">Represents the main gateway to interact with an app.</param>
        public static void TapMoreButton(this IApp app)
        {
            try
            {
                if (app is AppiumAndroidApp)
                {
                    app.Tap(AppiumQuery.ByXPath("//android.widget.ImageView[@content-desc=\"More options\"]"));
                }
                else if (app is AppiumWindowsApp)
                {
                    app.Tap(AppiumQuery.ByAccessibilityId("MoreButton"));
                }
            }
            catch (Exception ex)
            {
                LogException("TapMoreButton", ex);
                throw;
            }
        }

        /// <summary>
        /// Taps a tab in the application.
        /// </summary>
        /// <param name="app">The IApp instance representing the application.</param>
        /// <param name="tabName">The name of the tab to tap.</param>
        /// <param name="isTopTab">Indicates whether the tab is a top tab (default is false).</param>
        /// <remarks>
        /// This method handles platform-specific behaviors:
        /// - For Android, it converts the tab name to uppercase.
        /// - For Windows, if it's a top tab, it taps the navigation view item first.
        /// The method waits for the tab element to be available before tapping it.
        /// </remarks>
        public static void TapTab(this IApp app, string tabName, bool isTopTab = false)
        {
            try
            {
                tabName = app is AppiumAndroidApp ? tabName.ToUpperInvariant() : tabName;

                if (isTopTab && app is AppiumWindowsApp)
                {
                    app.WaitForElement("navViewItem");
                    app.Tap("navViewItem");
                }

                app.WaitForElementTillPageNavigationSettled(tabName);
                app.Tap(tabName);
            }
            catch (Exception ex)
            {
                LogException("TapTab", ex);
                throw;
            }
        }

        /// <summary>
        /// Waits for a tab element with the specified name to appear and for page navigation to settle.
        /// </summary>
        /// <param name="app">The IApp instance.</param>
        /// <param name="tabName">The name of the tab to wait for.</param>
        /// <remarks>
        /// For Android apps, the tab name is converted to uppercase before searching.
        /// </remarks>
        public static IUIElement WaitForTabElement(this IApp app, string tabName)
        {
            try
            {
                tabName = app is AppiumAndroidApp ? tabName.ToUpperInvariant() : tabName;
                return app.WaitForElementTillPageNavigationSettled(tabName);
            }
            catch (Exception ex)
            {
                LogException("WaitForTabElement", ex);
                throw;
            }
        }

        static IUIElement Wait(Func<IUIElement?> query,
            Func<IUIElement?, bool> satisfactory,
            string? timeoutMessage = null,
            TimeSpan? timeout = null, TimeSpan? retryFrequency = null)
        {
            try
            {
                timeout ??= DefaultTimeout;
                retryFrequency ??= TimeSpan.FromMilliseconds(500);
                timeoutMessage ??= "Timed out on query.";

                DateTime start = DateTime.Now;

                IUIElement? result = query();

                while (!satisfactory(result))
                {
                    long elapsed = DateTime.Now.Subtract(start).Ticks;
                    if (elapsed >= timeout.Value.Ticks)
                    {
                        Debug.WriteLine($">>>>> {elapsed} ticks elapsed, timeout value is {timeout.Value.Ticks}");

                        throw new TimeoutException(timeoutMessage);
                    }

                    Task.Delay(retryFrequency.Value.Milliseconds).Wait();
                    result = query();
                }

                return result!;
            }
            catch (Exception ex)
            {
                LogException("Wait", ex);
                throw;
            }
        }

        static IUIElement WaitForAtLeastOne(Func<IUIElement> query,
            string? timeoutMessage = null,
            TimeSpan? timeout = null,
            TimeSpan? retryFrequency = null)
        {
            try
            {
                var results = Wait(query, i => i != null, timeoutMessage, timeout, retryFrequency);

                return results;
            }
            catch (Exception ex)
            {
                LogException("WaitForAtLeastOne", ex);
                throw;
            }
        }

        static void WaitForNone(Func<IUIElement> query,
            string? timeoutMessage = null,
            TimeSpan? timeout = null, TimeSpan? retryFrequency = null)
        {
            try
            {
                Wait(query, i => i == null, timeoutMessage, timeout, retryFrequency);
            }
            catch (Exception ex)
            {
                LogException("WaitForNone", ex);
                throw;
            }
        }

        static IUIElement FindElement(IApp app, string element)
        {
            try
            {
                var result = app.FindElement(element);

                if (result is null)
                    result = app.FindElementByText(element);

                return result;
            }
            catch (Exception ex)
            {
                LogException("FindElement", ex);
                throw;
            }
        }

        static IUIElement FindAnyElement(IApp app, string[] elements)
        {
            try
            {
                foreach (var element in elements)
                {
                    if (FindElement(app, element) is IUIElement result)
                        return result;
                }

                throw new InvalidOperationException($"Did not find any elements in the list: {string.Join(", ", elements)}");
            }
            catch (Exception ex)
            {
                LogException("FindAnyElement", ex);
                throw;
            }
        }

        static IReadOnlyCollection<IUIElement> FindElements(IApp app, string element)
        {
            try
            {
                var result = app.FindElements(element);

                if (result is null)
                    result = app.FindElementsByText(element);

                return result;
            }
            catch (Exception ex)
            {
                LogException("FindElements", ex);
                throw;
            }
        }


        public static void SetTestConfigurationArg(this IConfig config, string key, string value)
        {
            try
            {
                var startupArg = config.GetProperty<Dictionary<string, string>>("TestConfigurationArgs") ?? new Dictionary<string, string>();
                startupArg.Add(key, value);
                config.SetProperty("TestConfigurationArgs", startupArg);
            }
            catch (Exception ex)
            {
                LogException("SetTestConfigurationArg", ex);
                throw;
            }
        }
        private static List<CustomPoint> points = new List<CustomPoint>();
        public static void TapByPointer(this IApp app, string pointerName)
        {
            try
            {

                var point = GetPointByName(pointerName);
                if (point == null)
                {
                    throw new Exception($"Point '{pointerName}' not found.");
                }
                else
                {
                    app.TapCoordinates(point.X, point.Y);
                }
            }

            catch (Exception ex)
            {
                LogException("TapByPointer", ex);
                throw;
            }
        }
        public static void AddPoint(this IApp app, string name, float x, float y)
        {
            var point = new CustomPoint(name, x, y);
            points.Add(point);
            Console.WriteLine($"Added: {point}");
        }

        public static List<CustomPoint> GetAllPoints(this IApp app)
        {
            return points;
        }

        public static void PrintAllPoints(this IApp app)
        {
            foreach (var point in points)
            {
                Console.WriteLine(point);
            }
        }
        public static CustomPoint GetPointByName(string name)
        {
            return points.FirstOrDefault(p => p.Name == name);
        }
    }
    public class CustomPoint
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public CustomPoint(string name, float x, float y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{Name}: ({X}, {Y})";
        }

    }
}