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
		private IRSAPIClient _client;

		private int _workspaceId;
		private string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		private string _testFieldName = "Production::Sort Order";
		private int _productionSortOrderFieldArtifactId;
		private int _productionSortOrderFieldCount = 1;

		[OneTimeSetUp]
		public void SetUp()
		{
			//ARRANGE

			_httpRequestHelper = new HttpRequestHelper();
			SuT = new FieldsHelper(_httpRequestHelper);
			_testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			_servicesManager = _testHelper.GetServicesManager();
			_client = _testHelper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create workspace
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			_client.APIOptions.WorkspaceID = _workspaceId;

			//Query for field ID to be used in test
			var fieldNameCondition = new TextCondition("Name", TextConditionEnum.EqualTo, _testFieldName);
			var query = new Query<RDO> { ArtifactTypeID = 14, Condition = fieldNameCondition };
			query.Fields.Add(new FieldValue("Artifact ID"));
			query.Fields.Add(new FieldValue("Name"));

			//Query and throw exception if the query fails
			var results = _client.Repositories.RDO.Query(query);
			if (!results.Success) throw new Exception("Failed to query for field.");
			_productionSortOrderFieldArtifactId = results.Results.First().Artifact.ArtifactID;

		}

		[OneTimeTearDown]
		public void Teardown()
		{
			//Delete workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_httpRequestHelper = null;
			SuT = null;
			_testHelper = null;
		}

		[Test]
		public void GetFieldArtifactIdTest()
		{
			//ACT
			var fieldArtifactId = SuT.GetFieldArtifactId(_testFieldName, _workspaceId);

			//ASSERT
			Assert.AreEqual(fieldArtifactId, _productionSortOrderFieldArtifactId);
		}

		[Test]
		public void GetFieldCountTest()
		{
			//ACT
			var fieldCount = SuT.GetFieldCount(_productionSortOrderFieldArtifactId, _workspaceId);

			//ASSERT
			Assert.AreEqual(fieldCount, _productionSortOrderFieldCount);
		}
	}
}
