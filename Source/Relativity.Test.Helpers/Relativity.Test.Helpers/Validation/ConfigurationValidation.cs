using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Validation
{
	public class ConfigurationValidation
	{
		public void ValidateRSAPIClient(TestHelper helper)
		{
			using (var client = helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				OperationResult opResult = client.ValidateEndpoint();
				if (!opResult.Success)
				{
					throw new IntegrationTestException($"RSAPIClient endpoint {client.EndpointUri} was not valid. Message was {opResult.Message}");
				}
				try
				{
					string token = client.Login();
				}
				catch (Exception ex)
				{
					throw new IntegrationTestException("RSAPIClient failed to log in.", ex);
				}
			}
		}
	}
}
