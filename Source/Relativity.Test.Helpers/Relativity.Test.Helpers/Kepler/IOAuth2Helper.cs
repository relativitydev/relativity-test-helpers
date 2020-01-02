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
		/// <returns></returns>
		Task<string> GetBearerTokenAsync(string protocol, string serverAddress, string clientId, string clientSecret, string scope = "SystemUserInfo", string grantType = "client_credentials");

		/// <summary>
		/// Use this to cleanup OAuth2 during tests
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		Task DeleteOAuth2ClientAsync(string clientId);
	}
}
