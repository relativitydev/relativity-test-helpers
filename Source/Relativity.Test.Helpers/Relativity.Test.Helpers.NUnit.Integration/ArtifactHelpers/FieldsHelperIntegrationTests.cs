using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class FieldsHelperIntegrationTests
	{
		private IFieldsHelper SuT;
		private IHttpRequestHelper _httpRequestHelper;
		private IHelper _testHelper;
		private IServicesMgr _servicesManager;
		private IRSAPIClient _rsapiClient;
		private IDBContext _dbContext;

		private int _workspaceId;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		private const string _testFieldName = "Production::Sort Order";
		private int _productionSortOrderFieldArtifactId;
		private const int _productionSortOrderFieldCount = 1;

		[OneTimeSetUp]
		public void SetUp()
		{
			//ARRANGE

			_testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			_servicesManager = _testHelper.GetServicesManager();
			_rsapiClient = _testHelper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create workspace
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			_rsapiClient.APIOptions.WorkspaceID = _workspaceId;

			//Query for field ID to be used in test
			var fieldNameCondition = new TextCondition("Name", TextConditionEnum.EqualTo, _testFieldName);
			var query = new Query<RDO> { ArtifactTypeID = 14, Condition = fieldNameCondition };
			query.Fields.Add(new FieldValue("Artifact ID"));
			query.Fields.Add(new FieldValue("Name"));

			//Query and throw exception if the query fails
			var results = _rsapiClient.Repositories.RDO.Query(query);
			if (!results.Success) throw new Exception("Failed to query for field.");
			_productionSortOrderFieldArtifactId = results.Results.First().Artifact.ArtifactID;

			_dbContext = _testHelper.GetDBContext(_workspaceId);

		}

		[OneTimeTearDown]
		public void Teardown()
		{
			//Delete workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = null;
			_rsapiClient = null;
			_dbContext = null;
			_testHelper = null;
		}

		[Test]
		public void GetFieldArtifactIdTest()
		{
			//ACT
			var fieldArtifactId = Fields.GetFieldArtifactID(_testFieldName, _dbContext);

			//ASSERT
			Assert.AreEqual(fieldArtifactId, _productionSortOrderFieldArtifactId);
		}

		[Test]
		public void GetFieldCountTest()
		{
			//ACT
			var fieldCount = Fields.GetFieldCount(_dbContext, _productionSortOrderFieldArtifactId);

			//ASSERT
			Assert.AreEqual(fieldCount, _productionSortOrderFieldCount);
		}
	}
}
