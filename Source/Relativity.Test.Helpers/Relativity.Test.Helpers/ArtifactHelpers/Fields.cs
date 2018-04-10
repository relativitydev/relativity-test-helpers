using kCura.Relativity.Client;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
    /// <summary>
    /// 
    /// Helpers to interact with Fields in Relativity
    /// 
    /// </summary>
    /// 
    public class Fields
	{
		public static Int32 GetFieldArtifactID(String fieldname, IDBContext workspaceDbContext)
		{
			string sqlquery = @"SELECT [ArtifactID] FROM [EDDSDBO].[Field] Where[DisplayName] like @fieldname";

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldname", SqlDbType.NVarChar) {Value = fieldname}
			};

			Int32 artifactTypeId = workspaceDbContext.ExecuteSqlStatementAsScalar<Int32>(sqlquery, sqlParams);

			return artifactTypeId;
		}

		public static Int32 GetFieldCount(IDBContext workspaceDbContext, int fieldArtifactId)
		{
			string sqlquery = String.Format(@"select count(*) from [EDDSDBO].[ExtendedField] where ArtifactID = @fieldArtifactId");

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldArtifactId", SqlDbType.NVarChar) {Value = fieldArtifactId}
			};

			int fieldCount = workspaceDbContext.ExecuteSqlStatementAsScalar<Int32>(sqlquery, sqlParams);

			return fieldCount;
		}



		public static Int32 CreateField_FixedLengthText(IRSAPIClient client, Int32 workspaceID)
		{
			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Read the object type to add fields to
			//kCura.Relativity.Client.DTOs.ObjectType objectToAddFieldTo = client.Repositories.ObjectType.ReadSingle(objectTypeID);

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();

			//Set primary fields
			//The name of the sample data is being set to a random string so that sample data can be debugged
			//and never causes collisions. You can set this to any string that you want
			fieldDTO.Name = string.Format("FixedLegthText{0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType()
			{
				DescriptorArtifactTypeID = (int)ArtifactType.Document
			};
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.FixedLengthText;

			//Set secondary fields
			fieldDTO.AllowHTML = false;
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.IncludeInTextIndex = true;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Length = 255;
			fieldDTO.Linked = false;
			fieldDTO.Unicode = true;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;
			fieldDTO.IsRelational = false;

			//Create the field
			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet =
				client.Repositories.Field.Create(fieldDTO);
			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;

				return fieldID;
			}
			else
			{
				throw new Exception("Field was not created");
				return fieldID;
			}
		}


		public static Int32 CreateField_LongText(IRSAPIClient client, int workspaceID)
		{

			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();

			//Set primary fields
			//The name of the sample data is being set to a random string so that sample data can be debugged
			//and never causes collisions. You can set this to any string that you want
			fieldDTO.Name = string.Format("API Sample {0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType()
			{
				DescriptorArtifactTypeID = (int)ArtifactType.Document
			};
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.LongText;

			// Set secondary fields
			fieldDTO.AllowHTML = false;
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.AvailableInViewer = false;
			fieldDTO.IncludeInTextIndex = true;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Linked = false;
			fieldDTO.Unicode = true;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;

			//Create the field
			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet = client.Repositories.Field.Create(fieldDTO);

			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				return fieldID;
			}
			else
			{
				Console.WriteLine("Field was not created");
				return fieldID;
			}
		}

		public static Int32 CreateField_WholeNumber(IRSAPIClient client, int workspaceID)
		{
			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();

			//Set primary fields
			//The name of the sample data is being set to a random string so that sample data can be debugged
			//and never causes collisions. You can set this to any string that you want
			fieldDTO.Name = string.Format("API Sample {0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType()
			{
				DescriptorArtifactTypeID = (int)ArtifactType.Document
			};
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.WholeNumber;

			// Set secondary fields
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Linked = false;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;

			//Create the field
			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet = client.Repositories.Field.Create(fieldDTO);

			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				return fieldID;
			}
			else
			{
				Console.WriteLine("Field was not created");
				return fieldID;
			}
		}

		public static Int32 CreateField_YesNO(IRSAPIClient client, int workspaceID)
		{
			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();

			//Set primary fields
			//The name of the sample data is being set to a random string so that sample data can be debugged
			//and never causes collisions. You can set this to any string that you want
			fieldDTO.Name = string.Format("API Sample {0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType()
			{
				DescriptorArtifactTypeID = (int)ArtifactType.Document
			};
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.YesNo;
			fieldDTO.YesValue = "Yes";
			fieldDTO.NoValue = "No";

			// Set secondary fields
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Linked = false;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;

			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet = client.Repositories.Field.Create(fieldDTO);

			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;

				return fieldID;
			}
			else
			{
				Console.WriteLine("Field was not created");
				return fieldID;
			}
		}


		public static Int32 CreateField_SingleChoice(IRSAPIClient client, int workspaceID)
		{
			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();
			fieldDTO.Name = string.Format("API Sample {0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType()
			{
				DescriptorArtifactTypeID = (int)ArtifactType.Document
			};
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.SingleChoice;

			// Set secondary fields
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.AvailableInFieldTree = false;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Linked = false;
			fieldDTO.Unicode = true;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;

			//Create the field
			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet =
				client.Repositories.Field.Create(fieldDTO);

			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				return fieldID;
			}
			else
			{
				Console.WriteLine("Field was not created");
				return fieldID;
			}
		}


		public static Int32 CreateField_MultipleChoice(IRSAPIClient client, int workspaceID)
		{
			Int32 fieldID = 0;

			//Set the workspace ID
			client.APIOptions.WorkspaceID = workspaceID;

			//Create a Field DTO
			kCura.Relativity.Client.DTOs.Field fieldDTO = new kCura.Relativity.Client.DTOs.Field();

			fieldDTO.Name = string.Format("API Sample {0}", Guid.NewGuid());
			fieldDTO.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType() { DescriptorArtifactTypeID = (int)ArtifactType.Document };
			fieldDTO.FieldTypeID = kCura.Relativity.Client.FieldType.MultipleChoice;

			// Set secondary fields
			fieldDTO.AllowGroupBy = false;
			fieldDTO.AllowPivot = false;
			fieldDTO.AllowSortTally = false;
			fieldDTO.AvailableInFieldTree = false;
			fieldDTO.IsRequired = false;
			fieldDTO.OpenToAssociations = false;
			fieldDTO.Linked = false;
			fieldDTO.Unicode = true;
			fieldDTO.Width = "";
			fieldDTO.Wrapping = true;

			//Create the fields
			kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet = client.Repositories.Field.Create(fieldDTO);

			//Check for success
			if (resultSet.Success)
			{
				fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				return fieldID;
			}
			else
			{
				Console.WriteLine("Field was not created");
				return fieldID;
			}
		}
	}
}