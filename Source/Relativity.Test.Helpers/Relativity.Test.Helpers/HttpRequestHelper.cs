using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.SharedTestHelpers;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Test.Helpers
{
	public class HttpRequestHelper: IHttpRequestHelper
	{
		public string SendPostRequest(RequestModel requestModel, string routeName)
		{
			string usernamePassword = string.Format("{0}:{1}", ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(usernamePassword));
			var restAddress = ConfigurationHelper.SERVER_BINDING_TYPE + "://" + ConfigurationHelper.REST_SERVER_ADDRESS + "/Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/" + routeName;

			HttpResponseMessage response;
			using (var httpClient = new HttpClient())
			{
				//Set httpClient Headers
				httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64UsernamePassword);
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);

				var jsonRequest = JsonConvert.SerializeObject(requestModel);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				response = httpClient.PostAsync(restAddress, content).Result;
			}

			if (!response.IsSuccessStatusCode)
			{
				throw new TestHelpersException($"Failed to hit endpoint {routeName}.");
			}

			var responseString = response.Content.ReadAsStringAsync().Result;
			return responseString;
		}
	}
}