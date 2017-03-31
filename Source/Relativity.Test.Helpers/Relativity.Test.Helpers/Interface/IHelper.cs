using Relativity.API;

namespace Relativity.Test.Helpers.Interface
{
	public interface IHelper
	{
		IServicesMgr GetServicesManager();
		IDBContext GetDBContext(int CaseID);

	}
}
