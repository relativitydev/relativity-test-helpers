using kCura.Relativity.Client;
using Newtonsoft.Json.Linq;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	public class OAuth2
	{
		/// <summary>
		/// Use this to create an OAuth2 authentication for Relativity, which will allow you to get a Bearer Token and gain access to Relativity
		/// </summary>
		/// <param name="oAuth2ClientManager"></param>
		/// <param name="rsapiClient"></param>
		/// <param name="username"></param>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		public static async Task<Services.Security.Models.OAuth2Client> CreateOAuth2ClientAsync(IOAuth2ClientManager oAuth2ClientManager, IRSAPIClient rsapiClient, string username, string oAuth2Name)
		{
			DTOs.User user = GetUser(rsapiClient, username);

			List<Services.Security.Models.OAuth2Client> oAuth = await oAuth2ClientManager.ReadAllAsync();
			if (oAuth.Exists(x => x.Name.Equals(oAuth2Name)))
			{
				if (oAuth.Exists(x => x.Name.Equals(oAuth2Name) && x.ContextUser == user.ArtifactID))
				{
					Console.WriteLine($"User ({user.EmailAddress}) already has OAuth2 Credentials within ({oAuth2Name})");
					return oAuth.Find(x => x.Name.Equals(oAuth2Name) && x.ContextUser == user.ArtifactID);
				}

				throw new Exception($"A different user was has OAuth2 Credentials within ({oAuth2Name})");
			}

			Services.Security.Models.OAuth2Client oAuthResult = await oAuth2ClientManager.CreateAsync(oAuth2Name, OAuth2Flow.ClientCredentials, new List<Uri>(), user.ArtifactID);
			if (oAuthResult != null)
			{
				Console.WriteLine($"Created OAuth2 Client Credentials for ({user.EmailAddress}) within ({oAuthResult.Name})");
				return oAuthResult;
			}
			else
			{
				throw new Exception($"Failed to create OAuth2 Client Credentials for ({user.EmailAddress}) within ({oAuth2Name})");
			}
		}

		/// <summary>
		/// Use this to get the Bearer Token that will give you access Relativity and various endpoints
		/// </summary>
		/// <param name="protocol"></param>
		/// <param name="serverAddress"></param>
		/// <param name="clientId"></param>
		/// <param name="clientSecret"></param>
		/// <param name="scope"></param>
		/// <param name="grantType"></param>
		/// <returns></returns>
		public static async Task<string> GetBearerTokenAsync(string protocol, string serverAddress, string clientId, string clientSecret, string scope = "SystemUserInfo", string grantType = "client_credentials")
		{
			string tokenUrl = GetTokenUrl(protocol, serverAddress);
			string tokenResponse = await GetTokenAsync(tokenUrl, clientId, clientSecret, scope, grantType);

			dynamic data = JObject.Parse(tokenResponse);
			string bearerToken = data.access_token.Value;
			return bearerToken;
		}

		/// <summary>
		/// Use this to cleanup OAuth2 during tests
		/// </summary>
		/// <param name="oAuth2ClientManager"></param>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public static async Task DeleteOAuth2ClientAsync(IOAuth2ClientManager oAuth2ClientManager, string clientId)
		{
			await oAuth2ClientManager.DeleteAsync(clientId);
		}

		/// <summary>
		/// Retrieves User info by email
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		private static DTOs.User GetUser(IRSAPIClient rsapiClient, string email)
		{
			rsapiClient.APIOptions = new APIOptions { WorkspaceID = -1 };

			DTOs.User user;

			Condition userQueryCondition = new TextCondition(DTOs.UserFieldNames.EmailAddress, TextConditionEnum.EqualTo, email);

			DTOs.Query<DTOs.User> userQuery = new DTOs.Query<DTOs.User>(DTOs.FieldValue.AllFields, userQueryCondition, new List<Sort>());

			DTOs.QueryResultSet<DTOs.User> resultSet = rsapiClient.Repositories.User.Query(userQuery);

			if (resultSet.Success && resultSet.TotalCount > 0)
			{
				user = resultSet.Results.First().Artifact;
			}
			else
			{
				throw new Exception($"Could not find user {email}");
			}

			return user;
		}

		/// <summary>
		/// Simply gets a bearer token from Relativity
		/// </summary>
		/// <param name="tokenUrl"></param>
		/// <param name="clientId"></param>
		/// <param name="clientSecret"></param>
		/// <param name="scope"></param>
		/// <param name="grantType"></param>
		/// <returns></returns>
		private static async Task<string> GetTokenAsync(string tokenUrl, string clientId, string clientSecret, string scope, string grantType)
		{
			HttpClient client = new HttpClient();
			var httpContent = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

			var values = new List<KeyValuePair<string, string>>();
			values.Add(new KeyValuePair<string, string>("client_id", clientId));
			values.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
			values.Add(new KeyValuePair<string, string>("scope", scope));
			values.Add(new KeyValuePair<string, string>("grant_type", grantType));

			httpContent.Content = new FormUrlEncodedContent(values);

			HttpResponseMessage response = await client.SendAsync(httpContent);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}

			Console.WriteLine("Token Generation failed.");

			return null;
		}

		/// <summary>
		/// Returns the URL for Token generation for oAuth
		/// </summary>
		/// <returns></returns>
		private static string GetTokenUrl(string protocol, string serverAddress)
		{
			return $"{protocol}://{serverAddress.ToLower().Replace("-services", "")}/Relativity/Identity/connect/token";
		}
	}
}
