using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.API.Context;
using Relativity.Kepler.Logging;
using Relativity.Services.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace TestHelpersKepler.Services.TestHelpersModule.v1
{
	public class TestHelpersService : ITestHelpersService
	{
		private IHelper _helper;
		private ILog _logger;

		// Note: IHelper and ILog are dependency injected into the constructor every time the service is called.
		public TestHelpersService(IHelper helper, ILog logger)
		{
			// Note: Set the logging context to the current class.
			_logger = logger.ForContext<TestHelpersService>();
			_helper = helper;
		}

		public async Task<GetGuidResponseModel> GetGuidAsync(int artifactID, int workspaceID)
		{
			try
			{
				Guid guid = await _helper.GetDBContext(workspaceID).ExecuteScalarAsync<Guid>(
					new ContextQuery
					{
						SqlStatement = "select ArtifactGuid from eddsdbo.ArtifactGuid where artifactId = @artifactId",
						Parameters = new[]
						{
							new SqlParameter("@artifactId", artifactID)
						}
					}).ConfigureAwait(false);

				GetGuidResponseModel ggModel = new GetGuidResponseModel()
				{
					Guid = guid
				};

				return ggModel;
			}
			catch (Exception exception)
			{
				// Note: logging templates should never use interpolation! Doing so will cause memory leaks.
				_logger.LogWarning(exception, "An exception occured during get guid for artifact {ArtifactId} in workspace {WorkspaceId}.", artifactID, workspaceID);

				// Throwing a user defined exception with a 404 status code.
				throw new TestHelpersServiceException($"An exception occured during get guid for artifact {artifactID} in workspace {workspaceID}.");
			}
		}

		public async Task<FieldArtifactIdResponseModel> GetFieldArtifactIdAsync(string fieldName, int workspaceId)
		{
			FieldArtifactIdResponseModel responseModel;

			try
			{
				int fieldId = await _helper.GetDBContext(workspaceId).ExecuteScalarAsync<int>(
					new ContextQuery
					{
						SqlStatement = "SELECT [ArtifactID] FROM [ExtendedField] WHERE [DisplayName] LIKE @fieldName",
						Parameters = new[]
						{
							new SqlParameter("@fieldName", fieldName)
						}
					}
					).ConfigureAwait(false);

				responseModel = new FieldArtifactIdResponseModel
				{
					ArtifactId = fieldId
				};
			}
			catch (Exception exception)
			{
				// Note: logging templates should never use interpolation! Doing so will cause memory leaks.
				_logger.LogWarning(exception,
					"An exception occured getting the Artifact ID for {fieldName} in workspace {WorkspaceId}.", fieldName, workspaceId);

				// Throwing a user defined exception with a 404 status code.
				throw new TestHelpersServiceException(
					$"An exception occured getting the Artifact ID for {fieldName} in workspace {workspaceId}.");
			}

			return responseModel;
		}


		public async Task<FieldCountResponseModel> GetFieldCountAsync(int artifactId, int workspaceId)
		{
			FieldCountResponseModel responseModel;

			try
			{
				int fieldCount = await _helper.GetDBContext(workspaceId).ExecuteScalarAsync<int>(
					new ContextQuery
					{
						SqlStatement = "SELECT COUNT(*) FROM [ExtendedField] WHERE ArtifactID = @fieldArtifactId",
						Parameters = new[]
						{
							new SqlParameter("@fieldArtifactId", artifactId)
						}
					}
				).ConfigureAwait(false);

				responseModel = new FieldCountResponseModel
				{
					Count = fieldCount
				};
			}
			catch (Exception exception)
			{
				// Note: logging templates should never use interpolation! Doing so will cause memory leaks.
				_logger.LogWarning(exception,
					"An exception occured getting the count of fields with ID {artifactId} in workspace {WorkspaceId}.", artifactId, workspaceId);

				// Throwing a user defined exception with a 404 status code.
				throw new TestHelpersServiceException(
					$"An exception occured getting the count of fields with ID {artifactId} in workspace {workspaceId}.");
			}

			return responseModel;
		}

		/// <summary>
		/// All Kepler services must inherit from IDisposable.
		/// Use this dispose method to dispose of any unmanaged memory at this point.
		/// See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose for examples of how to properly use the dispose pattern.
		/// </summary>
		public void Dispose()
		{ }
	}
}
