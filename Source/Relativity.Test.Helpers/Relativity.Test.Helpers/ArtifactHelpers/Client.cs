using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Choice;
using Relativity.Services.Client;
using System;
using System.Collections.Generic;
using Relativity.Services.ServiceProxy;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	public class Client
	{
		public static int Create_Client(IRSAPIClient client, Relativity.Services.ServiceProxy.ServiceFactory serviceFactory, string name)
		{
			var workspaceId = client.APIOptions.WorkspaceID;
			client.APIOptions.WorkspaceID = -1;

			using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
			{
				List<ChoiceRef> choiceRefs = proxy.GetStatusChoicesForClientAsync().Result;
				ChoiceRef statusRef = choiceRefs.Find(x => x.Name == "Active");
				var newClient = new Relativity.Services.Client.Client { Name = name, Number = Guid.NewGuid().ToString(), Status = statusRef, Keywords = "Temp Client", Notes = "Used in the Disable Inactve User Integration Test." };
				int clientArtifactId = proxy.CreateSingleAsync(newClient).Result;
				client.APIOptions.WorkspaceID = workspaceId;
				return clientArtifactId;
			}
		}

		public static void Delete_Client(Relativity.Services.ServiceProxy.ServiceFactory serviceFactory, int artifactId)
		{
			using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
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
