using Newtonsoft.Json;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using kCura.Vendor.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Relativity.Services.FieldManager;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	/// <summary>
	/// 
	/// Helpers to interact with Documents in Relativity
	/// 
	/// </summary>

	public class Document : IDocumentHelper
	{
		#region Public Methods

		public static string GetDocumentIdentifierFieldName(IServicesMgr svcMgr, int workspaceId, int fieldArtifactTypeId)
		{
			try
			{
				string fieldName = null;
				using (IObjectManager objectManager = svcMgr.GetProxy<IObjectManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef()
						{
							ArtifactTypeID = fieldArtifactTypeId
						},
						Fields = new List<FieldRef>()
						{
							new FieldRef {Name = "*"}
						},
						Condition = $"'Object Type' == 'Document'"
					};

					QueryResult queryResult = objectManager.QueryAsync(workspaceId, queryRequest, 0, 100).ConfigureAwait(false)
							.GetAwaiter().GetResult();
					if (queryResult.TotalCount == 0)
					{
						throw new Exception("Query for Document Fields returned no results");
					}

					foreach (RelativityObject obj in queryResult.Objects)
					{
						IEnumerable<FieldValuePair> isIdentifierfieldValuePair = obj.FieldValues.Where(x => x.Field.Name == "Is Identifier");
						IEnumerable<FieldValuePair> nameFieldValuePair = obj.FieldValues.Where(x => x.Field.Name == "Name");
						bool isIdentifierFieldValue = (bool)isIdentifierfieldValuePair.First().Value;
						if (isIdentifierFieldValue)
						{
							fieldName = nameFieldValuePair.First().Value.ToString();
							break;
						}
					}

					return fieldName;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to Query for Document Fields", ex);
			}
		}

		#endregion
	}
}
