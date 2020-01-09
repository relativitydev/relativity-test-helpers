using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public interface IApplicationInstallHelper
	{
		Task<int> InstallApplicationAsync(string applicationName, string rapFilePath, int workspaceId, bool unlockApps);
		Task<bool> DoesApplicationExistAsync(int workspaceId, string rapFileName);
	}
}
