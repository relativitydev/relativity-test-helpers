using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Test.Helpers.User
{
    public class DeleteUser
    {
        public static bool Delete_User(IRSAPIClient client, int artifactId)
        {
            client.APIOptions.WorkspaceID = -1;
			kCura.Relativity.Client.DTOs.User userToDelete = new kCura.Relativity.Client.DTOs.User(artifactId);
			WriteResultSet<kCura.Relativity.Client.DTOs.User> resultSet = new WriteResultSet<kCura.Relativity.Client.DTOs.User>();
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
