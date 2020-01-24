using Relativity.Kepler.Services;

namespace TestHelpersKepler.Interfaces.TestHelpersModule
{
	/// <summary>
	/// TestHelpersModule Module Interface.
	/// </summary>
	[ServiceModule("TestHelpersModule")]
	[RoutePrefix("TestHelpersModule", VersioningStrategy.Namespace)]
	public interface ITestHelpersModule
	{
	}
}