using Relativity.API;
using System;

namespace Relativity.Test.Helpers.Authentication
{
	public class AuthenticationManager : IAuthenticationMgr
	{
		public IUserInfo UserInfo { get; set; }
		public string AuthToken { get; set; }

		public AuthenticationManager(IUserInfo userinfo, string authToken = null)
		{
			if (userinfo == null)
			{
				throw new Exception("UserInfo cannot be null");
			}
			UserInfo = userinfo;
			AuthToken = authToken;
		}

		public string GetAuthenticationToken()
		{
			return AuthToken;
		}
	}
}
