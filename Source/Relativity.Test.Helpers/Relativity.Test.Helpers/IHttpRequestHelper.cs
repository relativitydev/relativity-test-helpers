using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers
{
	public interface IHttpRequestHelper<T>
	{
		HttpClient GetClient();

		string GetRestAddress(string routeName);

		StringContent GetRequestContent(T requestModel);

	}
}
