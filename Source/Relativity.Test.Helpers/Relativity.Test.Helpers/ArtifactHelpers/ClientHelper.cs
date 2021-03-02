using Relativity.API;
using Relativity.Services;
using Relativity.Services.Choice;
using Relativity.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	public class ClientHelper
	{
		public static int CreateClient(Services.ServiceProxy.ServiceFactory serviceFactory, string name)
		{
			try
			{
				using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
				{
					List<ChoiceRef> choiceRefs = proxy.GetStatusChoicesForClientAsync().ConfigureAwait(false).GetAwaiter().GetResult();
					ChoiceRef statusRef = choiceRefs.Find(x => x.Name == "Active");

					var newClient = new Services.Client.Client
					{
						Name = name,
						Number = Guid.NewGuid().ToString(),
						Status = statusRef,
						Keywords = "Test Client",
						Notes = "Created with Relativity Test Helpers."
					};

					var clientArtifactId = proxy.CreateSingleAsync(newClient).ConfigureAwait(false).GetAwaiter().GetResult();

					return clientArtifactId;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to create client.", ex);
			}
		}

		public static void DeleteClient(Services.ServiceProxy.ServiceFactory serviceFactory, int artifactId)
		{
			try
			{
				using (IClientManager proxy = serviceFactory.CreateProxy<IClientManager>())
				{
					proxy.DeleteSingleAsync(artifactId).ConfigureAwait(false).GetAwaiter().GetResult();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to delete client.", ex);
			}
		}

		public static async Task<int> GetClientId(IServicesMgr servicesMgr, string clientName)
		{
			int clientId;

			using (IClientManager clientManager = servicesMgr.CreateProxy<IClientManager>(ExecutionIdentity.CurrentUser))
			{
				Services.Query query = new Services.Query()
				{
					Condition = new TextCondition("Name", TextConditionEnum.EqualTo, clientName).ToQueryString()
				};
				ClientQueryResultSet result = await clientManager.QueryAsync(query);
				clientId = result.Results.First().Artifact.ArtifactID;
			}

			return clientId;
		}
	}
}
