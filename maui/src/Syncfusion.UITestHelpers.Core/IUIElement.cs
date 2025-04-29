namespace Syncfusion.UITestHelpers.Core
{
	public interface IUIElement : IUIElementQueryable
	{
		ICommandExecution Command { get; }
	}
}
