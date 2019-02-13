using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace Relativity.Test.Helpers.Authentication
{
	public class AuthenticationManager : IAuthenticationMgr
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
