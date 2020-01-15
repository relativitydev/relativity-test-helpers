using kCura.Relativity.Client;
using Newtonsoft.Json.Linq;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Kepler
{
	public class OAuth2Helper : IOAuth2Helper
	{
		private readonly IOAuth2ClientManager _oAuth2ClientManager;
		private readonly IRSAPIClient _rsapiClient;

		public OAuth2Helper(IOAuth2ClientManager oAuth2ClientManager, IRSAPIClient rsapiClient)
		{
			_oAuth2ClientManager = oAuth2ClientManager ?? throw new ArgumentNullException($"Parameter ({nameof(oAuth2ClientManager)}) cannot be null");
			_rsapiClient = rsapiClient ?? throw new ArgumentNullException($"Parameter ({nameof(rsapiClient)}) cannot be null");
		}

		/// <summary>
		/// Use this to create an OAuth2 authentication for Relativity, which will allow you to later get a Bearer Token and gain access to external Relativity endpoints
		/// </summary>
		/// <param name="username"></param>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		public async Task<Services.Security.Models.OAuth2Client> CreateOAuth2ClientAsync(string username, string oAuth2Name)
		{
			try
			{
				DTOs.User user = GetUser(username);
				Services.Security.Models.OAuth2Client oAuth2Client;

				List<Services.Security.Models.OAuth2Client> oAuth = await _oAuth2ClientManager.ReadAllAsync();
				if (oAuth.Exists(x => x.Name.Equals(oAuth2Name)))
				{
					if (oAuth.Exists(x => x.ContextUser == user.ArtifactID))
					{
						Console.WriteLine($"User ({user.EmailAddress}) already has OAuth2 Credentials within ({oAuth2Name})");

						oAuth2Client = oAuth.Find(x => x.Name.Equals(oAuth2Name) && x.ContextUser == user.ArtifactID);
					}
					else
					{
						throw new TestHelpersOAuth2Exception($"A different user was has OAuth2 Credentials within ({oAuth2Name})");
					}
				}
				else
				{
					Services.Security.Models.OAuth2Client oAuthResult = await _oAuth2ClientManager.CreateAsync(oAuth2Name, OAuth2Flow.ClientCredentials, new List<Uri>(), user.ArtifactID);
					if (oAuthResult != null)
					{
						Console.WriteLine($"Created OAuth2 Client Credentials for ({user.EmailAddress}) within ({oAuthResult.Name})");
						oAuth2Client = oAuthResult;
					}
					else
					{
						throw new TestHelpersOAuth2Exception($"Failed to create OAuth2 Client Credentials for ({user.EmailAddress}) within ({oAuth2Name})");
					}
				}

				return oAuth2Client;
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception($"Exception occurred in ({nameof(CreateOAuth2ClientAsync)})", ex);
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
		/// <param name="httpMessageHandler"></param>
		/// <returns></returns>
		public async Task<string> GetBearerTokenAsync(string protocol, string serverAddress, string clientId, string clientSecret, string scope = "SystemUserInfo", string grantType = "client_credentials", HttpMessageHandler httpMessageHandler = null)
		{
			try
			{
				string tokenUrl = GetTokenUrl(protocol, serverAddress);

				if (httpMessageHandler == null)
				{
					httpMessageHandler = new HttpClientHandler()
					{
						AllowAutoRedirect = false,
						UseDefaultCredentials = true
					};
				}

				string tokenResponse = await GetTokenAsync(tokenUrl, clientId, clientSecret, scope, grantType, httpMessageHandler);

				dynamic data = JObject.Parse(tokenResponse);
				string bearerToken = data.access_token.Value;

				return bearerToken;
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception($"Exception occurred in ({nameof(GetBearerTokenAsync)})", ex);
			}
		}

		/// <summary>
		/// Use this to cleanup OAuth2 after tests
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public async Task DeleteOAuth2ClientAsync(string clientId)
		{
			try
			{
				await _oAuth2ClientManager.DeleteAsync(clientId);
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception($"Exception occurred in ({nameof(DeleteOAuth2ClientAsync)})", ex);
			}
		}

		/// <summary>
		/// Returns the OAuth2 Client for a given Name, if it exists
		/// </summary>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		public async Task<Services.Security.Models.OAuth2Client> ReadOAuth2ClientAsync(string oAuth2Name)
		{
			try
			{
				List<Services.Security.Models.OAuth2Client> oAuth = await _oAuth2ClientManager.ReadAllAsync();

				if (oAuth.Exists(x => x.Name.Equals(oAuth2Name)))
				{
					return oAuth.Find(x => x.Name.Equals(oAuth2Name));
				}

				throw new TestHelpersOAuth2Exception($"Could not find an OAuth2 Client named ({oAuth2Name})");
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception($"Error reading OAuth2Client in {nameof(DoesOAuth2ClientExistAsync)}", ex);
			}
		}

		/// <summary>
		/// Checks to see if the OAuth2 Client already exists for a given Name
		/// </summary>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		public async Task<bool> DoesOAuth2ClientExistAsync(string oAuth2Name)
		{
			try
			{
				List<Services.Security.Models.OAuth2Client> oAuth = await _oAuth2ClientManager.ReadAllAsync();
				return oAuth.Exists(x => x.Name.Equals(oAuth2Name));
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception($"Failed to check for OAuth2 Clients existence in {nameof(DoesOAuth2ClientExistAsync)}", ex);
			}
		}

		/// <summary>
		/// Retrieves User info by email
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		private DTOs.User GetUser(string email)
		{
			try
			{
				_rsapiClient.APIOptions = new APIOptions { WorkspaceID = -1 };

				DTOs.User user;

				Condition userQueryCondition = new TextCondition(DTOs.UserFieldNames.EmailAddress, TextConditionEnum.EqualTo, email);

				DTOs.Query<DTOs.User> userQuery = new DTOs.Query<DTOs.User>(DTOs.FieldValue.AllFields, userQueryCondition, new List<Sort>());

				DTOs.QueryResultSet<DTOs.User> resultSet = _rsapiClient.Repositories.User.Query(userQuery);

				if (resultSet.Success && resultSet.TotalCount > 0)
				{
					user = resultSet.Results.First().Artifact;
				}
				else
				{
					throw new TestHelpersOAuth2Exception($"Error finding user {email}: {resultSet.Message}");
				}

				return user;
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception("Exception occurred in when attempting to retrieve a user by email", ex);
			}
		}

		/// <summary>
		/// Simply gets a bearer token from Relativity
		/// </summary>
		/// <param name="tokenUrl"></param>
		/// <param name="clientId"></param>
		/// <param name="clientSecret"></param>
		/// <param name="scope"></param>
		/// <param name="grantType"></param>
		/// <param name="httpMessageHandler"></param>
		/// <returns></returns>
		private async Task<string> GetTokenAsync(string tokenUrl, string clientId, string clientSecret, string scope, string grantType, HttpMessageHandler httpMessageHandler)
		{
			try
			{
				HttpClient client = new HttpClient(httpMessageHandler);
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

				throw new TestHelpersOAuth2Exception("Token Generation failed.");
			}
			catch (Exception ex)
			{
				throw new TestHelpersOAuth2Exception("Exception occurred in when attempting to generate the bearer token", ex);
			}
		}

		/// <summary>
		/// Returns the URL for Token generation for oAuth
		/// </summary>
		/// <returns></returns>
		private string GetTokenUrl(string protocol, string serverAddress)
		{
			// The removal of -services is just a pre-caution in case the user is passing in the RelOne Service address instead of just the instance address
			return $"{protocol}://{serverAddress.ToLower().Replace("-services", "")}/Relativity/Identity/connect/token";
		}
	}
}
