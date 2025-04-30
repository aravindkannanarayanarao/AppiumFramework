# Syncfusion UITest Helpers

## Overview

The Syncfusion UITest Helpers is a collection of libraries designed to facilitate automated UI testing for applications. This package includes helper libraries for Appium, core functionalities, NUnit integration, and screenshot capabilities.

## Libraries Included

- **Syncfusion.UITestHelpers.Appium**: A library that provides utilities for interacting with mobile applications using Appium.
- **Syncfusion.UITestHelpers.Core**: The core library that contains shared functionalities and utilities used across other libraries.
- **Syncfusion.UITestHelpers.NUnit**: A library that integrates NUnit testing framework with the UITest Helpers, providing additional testing capabilities.
- **Syncfusion.UITestHelpers.Screenshot**: A library that offers functionalities for capturing screenshots during tests.

## Installation

You can install the Syncfusion UITest Helpers NuGet package using the following command:

```
Install-Package Syncfusion.UITestHelpers
```

## Usage

### Appium Example

```csharp
using Syncfusion.UITestHelpers.Appium;

// Initialize Appium driver and perform actions
var appiumDriver = new AppiumDriver();
appiumDriver.LaunchApp();
```

### NUnit Example

```csharp
using NUnit.Framework;
using Syncfusion.UITestHelpers.NUnit;

[TestFixture]
public class UITest
{
    [Test]
    public void TestAppFunctionality()
    {
        // Test code using UITest Helpers
    }
}
```

## Contributing

Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.