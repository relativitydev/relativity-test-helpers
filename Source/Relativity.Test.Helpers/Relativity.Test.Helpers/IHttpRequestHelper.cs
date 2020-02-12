using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Test.Helpers
{
	public interface IHttpRequestHelper
	{
		string SendPostRequest(BaseRequestModel baseRequestModel, string routeName);
	}
}
