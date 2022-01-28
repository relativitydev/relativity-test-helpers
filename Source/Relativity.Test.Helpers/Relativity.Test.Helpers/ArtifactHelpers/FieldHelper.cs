using kCura.Vendor.Castle.Core.Internal;
using Relativity.API;
using Relativity.Services.Interfaces.Field;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using FieldType = Relativity.Services.Interfaces.Field.Models.FieldType;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	/// <summary>
	/// Helpers to interact with Fields in Relativity
	/// </summary>
	public class FieldHelper : IFieldsHelper
	{

		#region Public Methods

		public static int GetFieldArtifactID(IServicesMgr svcMgr, string fieldName, int workspaceId)
		{
			//if (fieldName.IsNullOrEmpty())
			//{
			//	throw new ArgumentNullException(nameof(fieldName), "Field name cannot be null or empty.");
			//}

			using (IObjectManager objectManager =
				svcMgr.GetProxy<IObjectManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
			{
				QueryRequest queryRequest = new QueryRequest()
				{
					ObjectType = new ObjectTypeRef()
					{
						ArtifactTypeID = 14
					},
					Fields = new List<FieldRef>()
					{
						new FieldRef {Name = "*"}
					},
					Condition = $"'Name' == '{fieldName}'"
				};

				QueryResult queryResult = objectManager.QueryAsync(workspaceId, queryRequest, 0, 100).ConfigureAwait(false).GetAwaiter().GetResult();
				if (queryResult.TotalCount == 0)
				{
					throw new Exception($"Query for Fields with name {fieldName} returned no results");
				}

				int fieldArtifactId = queryResult.Objects.First().ArtifactID;
				return fieldArtifactId;
			}
		}

		public static int GetFieldCount(IServicesMgr svcMgr, int fieldArtifactId, int workspaceId)
		{
			if (fieldArtifactId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fieldArtifactId), "Invalid Field Artifact Id");
			}

			using (IObjectManager objectManager =
				svcMgr.GetProxy<IObjectManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
			{
				QueryRequest queryRequest = new QueryRequest()
				{
					ObjectType = new ObjectTypeRef()
					{
						ArtifactTypeID = 14
					},
					Fields = new List<FieldRef>()
					{
						new FieldRef {Name = "*"}
					},
					Condition = $"'Artifact ID' == {fieldArtifactId}"
				};

				QueryResult queryResult = objectManager.QueryAsync(workspaceId, queryRequest, 0, 100).ConfigureAwait(false).GetAwaiter().GetResult();
				if (queryResult.TotalCount == 0)
				{
					throw new Exception($"Query for Fields with artifact id {fieldArtifactId} returned no results");
				}

				return queryResult.TotalCount;
			}
		}

		public static int CreateFieldDate(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new DateFieldRequest()
				{
					Name = $"{FieldType.Date}{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					Formatting = Services.Interfaces.Field.Models.Formatting.Date,
					OpenToAssociations = false,

					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateDateFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}

		public static int CreateFieldUser(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new UserFieldRequest()
				{
					Name = $"{FieldType.User}{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateUserFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}

		public static int CreateFieldFixedLengthText(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new FixedLengthFieldRequest()
				{
					Name = $"FixedLength{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					AllowHtml = false,
					IncludeInTextIndex = false,
					Length = 255,
					HasUnicode = true,
					IsRelational = false,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateFixedLengthFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}
		public static int CreateFieldLongText(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new LongTextFieldRequest()
				{
					Name = $"Long{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					AllowHtml = false,
					IncludeInTextIndex = false,
					AvailableInViewer = false,
					HasUnicode = true,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateLongTextFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}
		public static int CreateFieldWholeNumber(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new WholeNumberFieldRequest()
				{
					Name = $"{FieldType.WholeNumber}{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateWholeNumberFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}
		public static int CreateFieldYesNo(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new YesNoFieldRequest()
				{
					Name = $"{FieldType.YesNo}{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					DisplayValueTrue = "Yes",
					DisplayValueFalse = "No",
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateYesNoFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}
		public static int CreateFieldSingleChoice(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new SingleChoiceFieldRequest()
				{
					Name = $"Single{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					AvailableInFieldTree = false,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateSingleChoiceFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}
		public static int CreateFieldMultipleChoice(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				int fieldId;
				var fieldRequest = new MultipleChoiceFieldRequest()
				{
					Name = $"{FieldType.YesNo}{Guid.NewGuid()}",
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = 10 }, //document artifact type ID
					IsRequired = false,
					OpenToAssociations = false,
					IsLinked = false,
					FilterType = FilterType.None,
					AllowSortTally = true,
					Width = null,
					AllowGroupBy = false,
					AllowPivot = false,
					Wrapping = true,
					AvailableInFieldTree = false,
					RelativityApplications = new List<ObjectIdentifier>(),
					Keywords = "test helpers",
					Notes = "Created by Test Helpers"
				};

				using (IFieldManager fieldManager = servicesMgr.CreateProxy<IFieldManager>(ExecutionIdentity.CurrentUser))
				{
					fieldId = fieldManager.CreateMultipleChoiceFieldAsync(workspaceID, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating field.", ex);
			}
		}

		#endregion

	}
}