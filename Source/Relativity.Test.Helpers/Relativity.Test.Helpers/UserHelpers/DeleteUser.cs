using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Test.Helpers.UserHelpers
{
	public class DeleteUser
	{
		public static bool Delete_User(IRSAPIClient client, int artifactId)
		{
			client.APIOptions.WorkspaceID = -1;
			User userToDelete = new User(artifactId);
			WriteResultSet<kCura.Relativity.Client.DTOs.User> resultSet = new WriteResultSet<User>();
			try
			{
				resultSet = client.Repositories.User.Delete(userToDelete);

				if (!resultSet.Success || resultSet.Results.Count == 0)
				{
					throw new Exception("User was not found");
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine("An error occurred deleting for the user: {0}", ex.Message);
				return false;
			}

			return true;
		}

	}

}
