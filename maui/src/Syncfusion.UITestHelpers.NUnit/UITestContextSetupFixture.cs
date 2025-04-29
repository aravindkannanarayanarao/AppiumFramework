 
using NUnit.Framework;
using Syncfusion.UITestHelpers.Core;

[assembly: NonTestAssembly]

// SetupFixture runs once for all tests under the same namespace, if placed outside the namespace it will run once for all tests in the assembly
// Test assemblies that derive from this assembly will need to create a type that derives from this class (or duplicate the code herein)
[SetUpFixture]
public abstract class UITestContextSetupFixture
{
    public abstract IConfig GetTestConfig();
    protected static IServerContext? _serverContext;

	public static IServerContext ServerContext { get { return _serverContext ?? throw new InvalidOperationException($"Trying to get the {nameof(ServerContext)} before setup has run"); } }

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		Initialize();
	}

	[OneTimeTearDown]
	public void RunAfterAnyTests()
	{
		_serverContext?.Dispose();
		_serverContext = null;
        try
        {
            var testConfig = GetTestConfig();
            var appName = testConfig.GetProperty<string>("AppName");
            var process = System.Diagnostics.Process.GetProcessesByName(appName).FirstOrDefault();
            process?.Kill(); // Force close the application
        }
        catch (Exception ex)
        { 
            Console.WriteLine($"Failed to close the app: {ex.Message}");
        }
    }

	public abstract void Initialize();
}