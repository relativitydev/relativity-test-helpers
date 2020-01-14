using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public interface IApplicationInstallHelper
	{
		/// <summary>
		/// Installs the application into a workspace.  Will install the application into the Library if it does not exist there already
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="rapFilePath"></param>
		/// <param name="workspaceId"></param>
		/// <param name="unlockApps"></param>
		/// <returns></returns>
		Task<int> InstallApplicationAsync(string applicationName, string rapFilePath, int workspaceId, bool unlockApps);

		/// <summary>
		/// Deletes the application from the Library if it exists.
		/// </summary>
		/// <param name="applicationName"></param>
		/// <returns></returns>
		Task DeleteApplicationFromLibraryIfItExistsAsync(string applicationName);

		/// <summary>
		/// Checks if the application exists in the Library
		/// </summary>
		/// <param name="applicationName"></param>
		/// <returns></returns>
		Task<bool> DoesLibraryApplicationExistAsync(string applicationName);

		/// <summary>
		/// Checks if the application exists in the given workspace
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="workspaceId"></param>
		/// <param name="workspaceApplicationInstallId"></param>
		/// <returns></returns>
		Task<bool> DoesWorkspaceApplicationExistAsync(string applicationName, int workspaceId, int workspaceApplicationInstallId);
	}
}
