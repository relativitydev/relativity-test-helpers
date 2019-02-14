using Relativity.Services.Choice;
using Relativity.Services.Client;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.Objects.Client
{
	public class ClientHelper
	{
		private TestHelper _helper;

		public ClientHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public int Create(string name)
		{
			using (IClientManager proxy = _helper.GetServicesManager().CreateProxy<IClientManager>(API.ExecutionIdentity.System))
			{
				List<ChoiceRef> choiceRefs = proxy.GetStatusChoicesForClientAsync().Result;
				ChoiceRef statusRef = choiceRefs.Find(x => x.Name == "Active");
				var newClient = new Relativity.Services.Client.Client { Name = name, Number = Guid.NewGuid().ToString(), Status = statusRef, Keywords = "Temp Client", Notes = "Used in the Disable Inactve User Integration Test." };
				int clientArtifactId = proxy.CreateSingleAsync(newClient).Result;
				return clientArtifactId;
			}
		}

		public void Delete(int artifactId)
		{
			using (IClientManager proxy = _helper.GetServicesManager().CreateProxy<IClientManager>(API.ExecutionIdentity.System))
			{
				int j = 1;
				while (j <= 5)
				{
					try
					{
						proxy.DeleteSingleAsync(artifactId).Wait();
						j++;
						break;
					}
					catch (Exception ex)
					{
						if (j >= 5)
						{
							throw new Exception("Error deleting Client", ex);
						}
					}
				}
			}
		}
	}
}