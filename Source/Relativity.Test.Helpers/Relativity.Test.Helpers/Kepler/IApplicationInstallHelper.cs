using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public interface IApplicationInstallHelper
	{
		Task<int> InstallApplicationAsync(string applicationName, string rapFilePath, int workspaceId, bool unlockApps);
		Task DeleteApplicationFromLibraryAsync(string rapFileName);
		Task<bool> DoesLibraryApplicationExistAsync(string rapFileName);
		Task<bool> DoesWorkspaceApplicationExistAsync(string rapFileName, int workspaceId, int workspaceApplicationInstallId);
	}
}
