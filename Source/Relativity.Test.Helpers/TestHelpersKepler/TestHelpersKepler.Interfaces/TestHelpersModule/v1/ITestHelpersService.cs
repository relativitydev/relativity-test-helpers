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

		[HttpPost]
		[Route("GetGuid")]
		Task<GetGuidModel> GetGuidAsync(int artifactID, int workspaceID);
	}
}
