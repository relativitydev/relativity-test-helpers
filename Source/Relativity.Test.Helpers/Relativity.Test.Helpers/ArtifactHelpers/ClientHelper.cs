using System;
using System.Collections.Generic;
using Relativity.Services.Choice;
using Relativity.Services.Client;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	public class ClientHelper : IClientHelper
	{
		public int CreateClient(Services.ServiceProxy.ServiceFactory serviceFactory, string name)
		{
			try
			{
				using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
				{
					List<ChoiceRef> choiceRefs = proxy.GetStatusChoicesForClientAsync().Result;
					ChoiceRef statusRef = choiceRefs.Find(x => x.Name == "Active");

					var newClient = new Services.Client.Client
					{
						Name = name,
						Number = Guid.NewGuid().ToString(),
						Status = statusRef, Keywords = "Test Client",
						Notes = "Created with Relativity Test Helpers."
					};

					var clientArtifactId = proxy.CreateSingleAsync(newClient).Result;

					return clientArtifactId;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to create client.", ex);
			}
		}

		public void DeleteClient(Services.ServiceProxy.ServiceFactory serviceFactory, int artifactId)
		{
			try
			{
				using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
				{
					proxy.DeleteSingleAsync(artifactId).Wait();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to delete client.", ex);
			}
		}
	}
}
