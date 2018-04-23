using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;


namespace Relativity.Test.Helpers.GroupHelpers
{
    public static class DeleteGroup
	{

		public static bool Delete_Group(IRSAPIClient client, int artifactId)
		{
			var clientID = client.APIOptions.WorkspaceID;
			client.APIOptions.WorkspaceID = -1;
			Group groupToDelete = new Group(artifactId);
			WriteResultSet<Group> resultSet = new WriteResultSet<Group>();

			try
			{
				resultSet = client.Repositories.Group.Delete(groupToDelete);

				if (!resultSet.Success || resultSet.Results.Count == 0)
				{
					throw new Exception("Group not found in Relativity");
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("Group deletion threw an exception" + e);
				return false;
			}

			client.APIOptions.WorkspaceID = clientID;
			return true;
		}

	}
}
