using Relativity.API;

namespace Relativity.Test.Helpers.Authentication
{
	internal class AuthenticationManager : IAuthenticationMgr
	{
		public IUserInfo UserInfo { get; set; }
		public string AuthToken { get; set; }

		public AuthenticationManager(IUserInfo userinfo, string authToken = null)
		{
			UserInfo = userinfo;
			AuthToken = authToken;
		}

		public string GetAuthenticationToken()
		{
			return AuthToken;
		}
	}
}