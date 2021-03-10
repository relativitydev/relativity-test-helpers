using Relativity.API;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	public static class ChoiceHelper
	{
		public static async Task<int> GetChoiceId(IServicesMgr servicesMgr, string choiceName)
		{
			try
			{
				using (IObjectManager objectManager = servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.Choice },
						Condition = new TextCondition("Name", TextConditionEnum.EqualTo, choiceName).ToQueryString(),
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Name" }
						},
					};
					QueryResult result = await objectManager.QueryAsync(-1, queryRequest, 1, 10);

					return result.Objects.First().ArtifactID;
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"Could not find Choice in {nameof(GetChoiceId)} for {nameof(choiceName)} of {choiceName} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage);
			}
		}
	}
}
