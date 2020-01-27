using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers
{
	public class HttpRequestHelper<T> : IHttpRequestHelper<T>
	{
		public HttpClient GetClient()
		{
			HttpClient httpClient = new HttpClient();
			string usernamePassword =
				string.Format("{0}:{1}", ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(usernamePassword));

			//Set httpClient Headers
			httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64UsernamePassword);
			httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
			return httpClient;
		}

		public string GetRestAddress(string routeName)
		{
			return ConfigurationHelper.SERVER_BINDING_TYPE + "://" + ConfigurationHelper.REST_SERVER_ADDRESS + "/Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/" + routeName;
		}

		public StringContent GetRequestContent(T requestModel)
		{
			var jsonRequest = JsonConvert.SerializeObject(requestModel);
			return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
		}
	}
}