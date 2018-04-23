using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;

namespace Relativity.Test.Helpers.GroupHelpers
{
    public static class CreateGroup
	{
		public static int Create_Group(IRSAPIClient client, String name)
		{

			var clientID = client.APIOptions.WorkspaceID;
			client.APIOptions.WorkspaceID = -1;
			Group newGroup = new Group();
			newGroup.Name = name;
			WriteResultSet<kCura.Relativity.Client.DTOs.Group> resultSet = new WriteResultSet<Group>();

			try
			{
				resultSet = client.Repositories.Group.Create(newGroup);
				if (!resultSet.Success || resultSet.Results.Count == 0)
				{
					throw new Exception("Group was not found");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception occured while trying to create a new group" + e);
			}
			var groupartid = resultSet.Results[0].Artifact.ArtifactID;
			client.APIOptions.WorkspaceID = clientID;
			return groupartid;
		}

	}
}
