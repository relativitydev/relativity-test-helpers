using kCura.Relativity.Client;
using Relativity.API;
using DTOs = kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.Objects.RDO
{
	public class RDOHelper
	{
		private TestHelper _helper;

		public RDOHelper(TestHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Deletes an RDO by artifactID.
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="rdoArtifactID"></param>
		public void Delete(int workspaceID, int rdoArtifactID)
		{
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = workspaceID;
				client.Repositories.RDO.DeleteSingle(rdoArtifactID);
			}
		}

		/// <summary>
		/// Reads a single RDO by artifactID
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="rdoArtifactID"></param>
		/// <returns></returns>
		public DTOs.RDO ReadSingle(int workspaceID, int rdoArtifactID)
		{
			DTOs.RDO rdo;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = workspaceID;
				rdo = client.Repositories.RDO.ReadSingle(rdoArtifactID);
			}

			return rdo;
		}
	}
}