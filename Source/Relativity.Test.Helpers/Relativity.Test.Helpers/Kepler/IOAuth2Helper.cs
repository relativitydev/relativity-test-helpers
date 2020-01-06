using System.Net.Http;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public interface IOAuth2Helper
	{
		/// <summary>
		/// Use this to create an OAuth2 authentication for Relativity, which will allow you to get a Bearer Token and gain access to Relativity
		/// </summary>
		/// <param name="username"></param>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		Task<Services.Security.Models.OAuth2Client> CreateOAuth2ClientAsync(string username, string oAuth2Name);

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
		Task<string> GetBearerTokenAsync(string protocol, string serverAddress, string clientId, string clientSecret, string scope, string grantType, HttpMessageHandler httpMessageHandler);

		/// <summary>
		/// Use this to cleanup OAuth2 during tests
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		Task DeleteOAuth2ClientAsync(string clientId);

		/// <summary>
		/// Returns the OAuth2 Client for a given Name, if it exists
		/// </summary>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		Task<Services.Security.Models.OAuth2Client> ReadOAuth2ClientAsync(string oAuth2Name);

		/// <summary>
		/// Checks to see if the OAuth2 Client already exists for a given Name
		/// </summary>
		/// <param name="oAuth2Name"></param>
		/// <returns></returns>
		Task<bool> DoesOAuth2ClientExistAsync(string oAuth2Name);
	}
}
