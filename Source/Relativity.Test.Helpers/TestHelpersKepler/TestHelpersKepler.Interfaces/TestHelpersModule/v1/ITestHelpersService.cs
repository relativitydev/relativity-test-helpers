using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.Kepler.Services;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace TestHelpersKepler.Interfaces.TestHelpersModule.v1
{
	/// <summary>
	/// MyService Service Interface.
	/// </summary>
	[WebService("TestHelpersService Service")]
	[ServiceAudience(Audience.Public)]
	[RoutePrefix("TestHelpersService")]
	public interface ITestHelpersService : IDisposable
	{
		/// <summary>
		/// Get the guid of an artifact in a specific workspace
		/// </summary>
		/// <param name="artifactID">The artifactID for an artifact you want the guid of.</param>
		/// <param name="workspaceID">The workspaceID of the workspace where the artifact exists</param>
		/// <returns><see cref="GetGuidResponseModel"/> containing the guid of the artifact.</returns>
		/// <remarks>
		/// Example REST request:
		///   [POST] /Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/GetGuid
		///   { "artifactID":"12345", "workspaceID":"-1" }
		/// Example REST response:
		///   {"Guid":"f5d53469-9211-4ba3-bb4d-d33ae9b0634c"}
		/// </remarks>
		[HttpPost]
		[Route("GetGuid")]
		Task<GetGuidResponseModel> GetGuidAsync(int artifactID, int workspaceID);

		[HttpPost]
		[Route("GetFieldArtifactId")]
		Task<FieldArtifactIdResponseModel> GetFieldArtifactIdAsync(string fieldName, int workspaceId);

		[HttpPost]
		[Route("GetFieldCount")]
		Task<FieldCountResponseModel> GetFieldCountAsync(int fieldArtifactId, int workspaceId);

		[HttpPost]
		[Route("GetFolderName")]
		Task<GetFolderNameResponseModel> GetFolderNameAsync(int folderArtifactId, int workspaceId);

		[HttpPost]
		[Route("GetDocumentIdentifierFieldColumnName")]
		Task<GetDocumentIdentifierFieldColumnNameResponseModel> GetDocumentIdentifierFieldColumnName(int fieldArtifactTypeId, int workspaceId);

		[HttpPost]
		[Route("GetDocumentIdentifierFieldName")]
		Task<GetDocumentIdentifierFieldNameResponseModel> GetDocumentIdentifierFieldName(int fieldArtifactTypeId, int workspaceId);
	}
}
