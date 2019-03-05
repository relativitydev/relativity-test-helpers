using Newtonsoft.Json;
using Relativity.API;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using Relativity.Test.Helpers.Configuration.Models;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using AuthClient = Relativity.Services.Security.Models.OAuth2Client;
using Relativity.Test.Helpers.Objects.OAuth2Client.Models;

namespace Relativity.Test.Helpers.Objects.OAuth2Client
{
	public class OAuth2ClientHelper
	{
		private TestHelper _helper;

		public OAuth2ClientHelper(TestHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Creates OAuth2Client or reads existing OAuth2Client
		/// </summary>
		/// <param name="shouldCreateOAuth2Client"></param>
		/// <returns></returns>
		public AuthClient Create(bool shouldCreateOAuth2Client = true)
		{
			AuthClient clientToCreate;
			using (var authManager = _helper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System))
			{
				var oauth2Client = new AuthClient
				{
					Id = "2200755ff6c5fbe7d5275fb683",
					Name = "Integration Test Client",
					Flow = OAuth2Flow.ClientCredentials,
					AccessTokenLifetimeInMinutes = 60,
					Enabled = true,
					ContextUser = 9 // Rel Admin
				};
				if (shouldCreateOAuth2Client)
				{
					clientToCreate = authManager.CreateAsync(oauth2Client).Result;
				}
				else
				{
					try
					{
						clientToCreate = authManager.ReadAsync(oauth2Client.Id).Result;
					}
					catch (Exception ex)
					{
						throw new IntegrationTestException("The integration test must have an OAuth2Client present in the remote instance.");
					}
				}
			}
			return clientToCreate;
		}

		/// <summary>
		/// Returns an auth token by clientID and client secret.
		/// </summary>
		/// <param name="configs"></param>
		/// <param name="clientID"></param>
		/// <param name="clientSecret"></param>
		/// <returns></returns>
		public string ReadToken(ConfigurationModel configs, string clientID, string clientSecret)
		{
			string authToken;
			var requestString = String.Format("{0}://{1}/Relativity/Identity/connect/token", configs.ServerHostBinding, configs.ServerHostName);

			var payload = new List<KeyValuePair<string, string>>
						{
								new KeyValuePair<string, string>("client_id", clientID),
								new KeyValuePair<string, string>("client_secret", clientSecret),
								new KeyValuePair<string, string>("scope", "SystemUserInfo"),
								new KeyValuePair<string, string>("grant_type", "client_credentials")
						};

			var request = new HttpRequestMessage
			{
				RequestUri = new Uri(requestString),
				Content = new FormUrlEncodedContent(payload),
				Method = HttpMethod.Post
			};

			using (HttpClient client = new HttpClient())
			{
				HttpResponseMessage response = client.SendAsync(request).Result;
				var contentBody = response.Content.ReadAsStringAsync().Result;
				var content = JsonConvert.DeserializeObject<AuthContent>(contentBody);
				authToken = content.access_token;
			}

			return authToken;
		}
	}
}