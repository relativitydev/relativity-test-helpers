using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public interface IApplicationInstallHelper
	{
		Task<int> InstallApplicationAsync(string applicationName, string rapFilePath, int workspaceId, bool unlockApps);
		Task DeleteApplicationFromLibraryIfItExistsAsync(string applicationName);
		Task<bool> DoesLibraryApplicationExistAsync(string applicationName);
		Task<bool> DoesWorkspaceApplicationExistAsync(string applicationName, int workspaceId, int workspaceApplicationInstallId);
	}
}
