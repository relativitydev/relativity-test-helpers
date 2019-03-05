using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Services.Matter;
using Relativity.API;
using kCura.Relativity.Client;
using Relativity.Test.Helpers.Exceptions;

namespace Relativity.Test.Helpers.Objects.Matter
{
	public class MatterHelper
	{
		private TestHelper _helper;

		public MatterHelper(TestHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Creates a matter with a random number and active status.
		/// </summary>
		/// <param name="matterName"></param>
		/// <param name="clientArtifactID"></param>
		/// <returns></returns>
		public int Create(string matterName, int clientArtifactID)
		{
			int matterArtifactID;

			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				var matterDTO = new Services.Matter.Matter
				{
					Name = matterName,
					Client = new Relativity.Services.Client.ClientRef(clientArtifactID),
					Number = new Random().Next(1000).ToString(),
					Status = new Relativity.Services.Choice.ChoiceRef(671),
					Notes = "Integration Test Matter"
				};

				matterArtifactID = matterManager.CreateSingleAsync(matterDTO).Result;
			}
			return matterArtifactID;
		}

		/// <summary>
		/// Returns the first matter artifact ID that matches the name provided.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int QueryMatterIDByName(string name)
		{
			int matterID;
			MatterQueryResultSet results;

			var query = new Services.Query
			{
				Condition = $"('Name' == '{name}')"
			};

			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				results = matterManager.QueryAsync(query).Result;
			}

			if (results.Success)
			{
				matterID = results.Results[0].Artifact.ArtifactID;
			}
			else
			{
				throw new IntegrationTestException($"Failed to retrieve matter by name equal to {name}");
			}

			return matterID;
		}

		/// <summary>
		/// Deletes the matter by artifact ID.
		/// </summary>
		/// <param name="matterID"></param>
		public void Delete(int matterID)
		{
			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				matterManager.DeleteSingleAsync(matterID).Wait();
			}
		}

	}
}
