using Relativity.API;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Authentication
{

    /// <summary>
    /// 
    /// Authentication Providers will help you add an Authentication Provider to a user in your instance of Relativity. This is extremely beneficial when you are trying to write permission tests around your application.
    /// 
    /// Caveat with this helper is that when the RSAPI creates a user, Password Authentication provider automatically gets added to the user. If that is something you dont want, you have to remove the provider type from the user using
    /// the RemoveProviderPasswordfromUser(...) method and then add the desired Provider type.
    /// 
    /// </summary>
    /// 
    public class AuthenticationProvider
    {
		private IHelper _helper;
		public AuthenticationProvider(IHelper helper)
		{
			_helper = helper;
		}

        public List<string> AuthenticationProviderPassword(Int32 userartifactid, string emailaddress, string twofactorMode)
        {
            List<String> violations = new List<string>();
        
            try
            { 
                var loginmanager = LoginProfileManager();

                var userprofile = loginmanager.GetLoginProfileAsync(userartifactid).Result;

                if (twofactorMode == "None")
                {
                    userprofile.Password = new Relativity.Services.Security.Models.PasswordMethod()
                    {
                        IsEnabled = true,
                        MustResetPasswordOnNextLogin = false,
                        PasswordExpirationInDays = 30,
                        UserCanChangePassword = true,
                        TwoFactorMode = Relativity.Services.Security.Models.TwoFactorMode.None
                        //Required for Two Factor Always And Outside Ips
                        //TwoFactorInfo = emailaddress
                    };
                }
                else if (twofactorMode == "Always")
                {
                    userprofile.Password = new Relativity.Services.Security.Models.PasswordMethod()
                    {
                        IsEnabled = true,
                        MustResetPasswordOnNextLogin = false,
                        PasswordExpirationInDays = 30,
                        UserCanChangePassword = true,
                        TwoFactorMode = Relativity.Services.Security.Models.TwoFactorMode.Always,
                        //Required for Two Factor Always And Outside Ips
                        TwoFactorInfo = emailaddress
                    };
                }
                else if (twofactorMode == "Outside Trusted Ips")
                {
                    userprofile.Password = new Relativity.Services.Security.Models.PasswordMethod()
                    {
                        IsEnabled = true,
                        MustResetPasswordOnNextLogin = false,
                        PasswordExpirationInDays = 30,
                        UserCanChangePassword = true,
                        TwoFactorMode = Relativity.Services.Security.Models.TwoFactorMode.OutsideResistrectedIPs,
                        //Required for Two Factor Always And Outside Ips
                        TwoFactorInfo = emailaddress
                    };
                }

                loginmanager.SaveLoginProfileAsync(userprofile).Wait();
            }
            catch (Exception e)
            {
                violations.Add(e.ToString());
            }
            return violations;
        }


        public List<string> RemoveProviderPasswordfromUser(Int32 userartifactid)
        {
            List<String> violations = new List<string>();
            try
            {
                var loginmanager = LoginProfileManager();

                var userprofile = loginmanager.GetLoginProfileAsync(userartifactid).Result;

                userprofile.Password = null;

                loginmanager.SaveLoginProfileAsync(userprofile).Wait();
            }
            catch (Exception e)
            {
                violations.Add(e.ToString());
            }
            return violations;
        }


        public List<string> AuthenticationProviderActiveDirectory(Int32 userartifactid, string emailaddress)
        {
            List<String> violations = new List<string>();
            try
            {
                var loginmanager = LoginProfileManager();

                var userprofile = loginmanager.GetLoginProfileAsync(userartifactid).Result;

                //Active Directory
                userprofile.ActiveDirectory = new Relativity.Services.Security.Models.ActiveDirectoryMethod()
                {
                    IsEnabled = true,
                    Account = emailaddress
                };

                loginmanager.SaveLoginProfileAsync(userprofile).Wait();
            }
            catch (Exception e)
            {
                violations.Add(e.ToString());
            }
            return violations;
        }


        public async Task<LoginProfile> RetrieveExistingUserLoginProfileAsync(Int32 userArtifactId)
        {
            try
            {
                var loginmanager = LoginProfileManager();
                LoginProfile userprofile = await loginmanager.GetLoginProfileAsync(userArtifactId);
                return userprofile;
            }
            catch (Exception ex)
            {
                throw new Exception("Cant retrieve user login information" + ex.ToString());
            }
        }

        public List<string> AuthenticationProviderIntegratedAuth(Int32 userartifactid, string account)
        {
            List<String> violations = new List<string>();
            try
            {
                var loginmanager = LoginProfileManager();

                var userprofile = loginmanager.GetLoginProfileAsync(userartifactid).Result;

                //Integrated Auth
                userprofile.IntegratedAuthentication = new IntegratedAuthenticationMethod()
                {
                    IsEnabled = true,
                    Account = account
                };

                loginmanager.SaveLoginProfileAsync(userprofile).Wait();
            }
            catch (Exception e)
            {
                violations.Add(e.ToString());
            }
            return violations;

        }

        private ILoginProfileManager LoginProfileManager()
        {
            ILoginProfileManager loginmanager = _helper.GetServicesManager().CreateProxy<ILoginProfileManager>(API.ExecutionIdentity.System);
            return loginmanager;
        }
    }
}
