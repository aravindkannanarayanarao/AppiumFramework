namespace Syncfusion.UITestHelpers.Core
{
    public interface IServerContext : IDisposable
	{
		IUIClientContext CreateUIClientContext(IConfig config);
	}
}
