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
		/// Get workspace name.
		/// </summary>
		/// <param name="workspaceID">Workspace ArtifactID.</param>
		/// <returns><see cref="TestHelpersServiceModel"/> with the name of the workspace.</returns>
		/// <remarks>
		/// Example REST request:
		///   [GET] /Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/workspace/1015024
		/// Example REST response:
		///   {"Name":"Relativity Starter Template"}
		/// </remarks>
		[HttpGet]
		[Route("workspace/{workspaceID:int}")]
		Task<TestHelpersServiceModel> GetWorkspaceNameAsync(int workspaceID);

		/// <summary>
		/// Query for a workspace by name
		/// </summary>
		/// <param name="queryString">Partial name of a workspace to query for.</param>
		/// <param name="limit">Limit the number of results via a query string parameter. (Default 10)</param>
		/// <returns>Collection of <see cref="TestHelpersServiceModel"/> containing workspace names that match the query string.</returns>
		/// <remarks>
		/// Example REST request:
		///   [POST] /Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/workspace?limit=2
		///   { "queryString":"a" }
		/// Example REST response:
		///   [{"Name":"New Case Template"},{"Name":"Relativity Starter Template"}]
		/// </remarks>
		[HttpPost]
		[Route("workspace?{limit}")]
		Task<List<TestHelpersServiceModel>> QueryWorkspaceByNameAsync(string queryString, int limit = 10);


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
	}
}
