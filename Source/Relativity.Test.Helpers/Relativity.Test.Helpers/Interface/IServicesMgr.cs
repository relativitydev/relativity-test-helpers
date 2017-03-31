using System;

namespace Relativity.Test.Helpers.Interface
{
	public interface IServicesMgr
	{

		T GetProxy<T>(string username, string password) where T : IDisposable;

	}
}
