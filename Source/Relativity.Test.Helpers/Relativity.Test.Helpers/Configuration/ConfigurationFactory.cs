using Relativity.Test.Helpers.Configuration.Models;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Configuration
{
	public class ConfigurationFactory
	{
		public static ConfigurationModel ReadConfigFromAppSettings(string tagSectionName = "appSettings")
		{
				var localSection = (NameValueCollection)ConfigurationManager.GetSection(tagSectionName);
				ConfigurationModel configs;
				try
				{
					configs = new ConfigurationModel()
					{
						WorkspaceID = int.Parse(localSection["WorkspaceID"]),
						WorkspaceName = localSection["WorkspaceName"],
						WorkspaceTemplateName = localSection["WorkspaceTemplateName"],
						AdminUsername = localSection["AdminUsername"],
						AdminPassword = localSection["AdminPassword"],
						SQLServerAddress = localSection["SQLServerAddress"],
						ServerHostName = localSection["ServerHostName"],
						ServerHostBinding = localSection["ServerHostBinding"],
						SQLUserName = localSection["SQLUserName"],
						SQLPassword = localSection["SQLPassword"],
						ApplicationRAPPath = localSection["ApplicationRAPPath"],
						RAPFilesDirectory = localSection["RAPFilesDirectory"],
						EmailAddress = localSection["EmailTo"]
					};
				}
				catch (ArgumentNullException ex)
				{

					throw new IntegrationTestException($"Section parameter was null. Please ensure app.config has all necessary keys", ex);
				}

				return configs;
		}
	}
}
