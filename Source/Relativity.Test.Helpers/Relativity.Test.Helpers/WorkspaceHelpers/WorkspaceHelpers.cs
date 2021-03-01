using Relativity.API;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using NumericConditionEnum = Relativity.Services.NumericConditionEnum;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{
	public class WorkspaceHelpers
	{
		public static string GetWorkspaceName(IServicesMgr servicesMgr, int workspaceArtifactId)
		{
			try
			{
				using (IObjectManager objectManager = servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.Workspace },
						Condition = new Relativity.Services.WholeNumberCondition("ArtifactId", NumericConditionEnum.EqualTo, workspaceArtifactId).ToQueryString(),
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Name" }
						},
					};
					QueryResult result = objectManager.QueryAsync(-1, queryRequest, 1, 10).Result;

					return result.Objects.First().FieldValues.Find(x => x.Field.Name == "Name").Value.ToString();
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"Could not find workspace name in {nameof(GetWorkspaceName)} for {nameof(workspaceArtifactId)} of {workspaceArtifactId} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage);
			}

		}
	}
}
