namespace Syncfusion.UITestHelpers.Core
{
	public interface IUIClientContext : IDisposable
	{
		public IApp App { get; }
		public IConfig Config { get; }
	}
}
