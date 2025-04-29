using Syncfusion.UITestHelpers.Core;
namespace Syncfusion.UITestHelpers.Screenshot;
public static class PlatformHelper
{
    public static TestDevice GetCurrentTestDevice()
    {
#if ANDROID
        return TestDevice.Android;
#elif IOS
        return TestDevice.iOS;
#elif MACCATALYST || MACOS
        return TestDevice.Mac;
#elif WINDOWS
        return TestDevice.Windows;
#else
        throw new PlatformNotSupportedException("Unsupported platform.");
#endif
    }
}
