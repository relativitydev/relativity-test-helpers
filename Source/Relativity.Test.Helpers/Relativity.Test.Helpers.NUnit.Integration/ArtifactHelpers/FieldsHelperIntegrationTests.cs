﻿using System;
using System.Collections.Generic;
using System.Linq;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;
using FieldType = Relativity.Services.Interfaces.Field.Models.FieldType;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class FieldsHelperIntegrationTests
	{
		private IHelper _testHelper;
		private IServicesMgr _servicesManager;
		private IRSAPIClient _rsapiClient;
		private IDBContext _dbContext;

		private int _workspaceId;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		private const string _testFieldName = "Production::Sort Order";
		private int _productionSortOrderFieldArtifactId;
		private const int _productionSortOrderFieldCount = 1;

		private KeplerHelper _keplerHelper;
		private bool useDbContext;

		private Services.ServiceProxy.ServiceFactory _serviceFactory;

		[OneTimeSetUp]
		public void SetUp()
		{
			//ARRANGE
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);
			_servicesManager = _testHelper.GetServicesManager();
			_rsapiClient = _testHelper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_serviceFactory = GetServiceFactory();

			//Create workspace
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).ConfigureAwait(false).GetAwaiter().GetResult();
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

			_keplerHelper = new KeplerHelper();
			bool isKeplerCompatible = _keplerHelper.IsVersionKeplerCompatibleAsync().ConfigureAwait(false).GetAwaiter().GetResult();
			useDbContext = !isKeplerCompatible || ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
			if (useDbContext)
			{
				_dbContext = _testHelper.GetDBContext(_workspaceId);
			}
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			//Delete workspace
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = null;
			_rsapiClient = null;
			_dbContext = null;
			_testHelper = null;
			_serviceFactory = null;
		}

		[Test]
		public void GetFieldArtifactIdTest()
		{
			//ACT
			int fieldArtifactId;
			if (useDbContext)
			{
				fieldArtifactId = FieldHelper.GetFieldArtifactID(_testFieldName, _dbContext);
			}
			else
			{
				fieldArtifactId = FieldHelper.GetFieldArtifactID(_testFieldName, _workspaceId, _keplerHelper);
			}

			//ASSERT
			Assert.AreEqual(fieldArtifactId, _productionSortOrderFieldArtifactId);
		}

		[Test]
		public void GetFieldArtifactIdTest_Invalid()
		{
			//ACT
			var invalidFieldName = "";

			//ASSERT
			if (useDbContext)
			{
				Assert.Throws<ArgumentNullException>(() => FieldHelper.GetFieldArtifactID(invalidFieldName, _dbContext));
			}
			else
			{
				int fieldArtifactId = FieldHelper.GetFieldArtifactID(invalidFieldName, _workspaceId, _keplerHelper);
				Assert.AreEqual(0, fieldArtifactId);
			}
		}

		[Test]
		public void GetFieldCountTest()
		{
			//ACT
			int fieldCount;
			if (useDbContext)
			{
				fieldCount = FieldHelper.GetFieldCount(_dbContext, _productionSortOrderFieldArtifactId);
			}
			else
			{
				fieldCount = FieldHelper.GetFieldCount(_productionSortOrderFieldArtifactId, _workspaceId, _keplerHelper);
			}

			//ASSERT
			Assert.AreEqual(fieldCount, _productionSortOrderFieldCount);
		}

		[Test]
		public void GetFieldCountTest_Invalid()
		{
			//ACT
			var invalidfieldArtifactId = -1;

			//ASSERT
			if (useDbContext)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => FieldHelper.GetFieldCount(_dbContext, invalidfieldArtifactId));
			}
			else
			{
				int fieldCount = FieldHelper.GetFieldCount(invalidfieldArtifactId, _workspaceId, _keplerHelper);
				Assert.AreEqual(0, fieldCount);
			}
		}

		[Test]
		public void CreateFieldDateTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldDate(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.Date);
		}

		[Test]
		public void CreateFieldUserTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldUser(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.User);
		}

		[Test]
		public void CreateFieldFixedLengthTextTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldFixedLengthText(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.FixedLength);
		}

		[Test]
		public void CreateFieldLongTextTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldLongText(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.LongText);

		}

		[Test]
		public void CreateFieldWholeNumberTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldWholeNumber(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.WholeNumber);
		}

		[Test]
		public void CreateFieldYesNoTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldYesNo(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.YesNo);
		}

		[Test]
		public void CreateFieldSingleChoiceTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldSingleChoice(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.SingleChoice);
		}

		[Test]
		public void CreateFieldMultipleChoiceTest()
		{
			//Act
			var fieldId = FieldHelper.CreateFieldMultipleChoice(_serviceFactory, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.MultipleChoice);
		}

		private FieldType ReadField(int fieldId, int workspaceId)
		{
			using (var fieldManager = _servicesManager.GetProxy<Services.Interfaces.Field.IFieldManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
			{
				var response = fieldManager.ReadAsync(workspaceId, fieldId).ConfigureAwait(false).GetAwaiter().GetResult();
				var fieldType = response.FieldType;
				return fieldType;
			}
		}

		//helper method
		private Services.ServiceProxy.ServiceFactory GetServiceFactory()
		{
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativity.Services");
			var relativityRestUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS.ToLower().Replace("-services", "")}/Relativity.Rest/Api");

			Relativity.Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(
				username: ConfigurationHelper.ADMIN_USERNAME,
				password: ConfigurationHelper.DEFAULT_PASSWORD);

			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				relativityServicesUri: relativityServicesUri,
				relativityRestUri: relativityRestUri,
				credentials: usernamePasswordCredentials);

			var serviceFactory = new Services.ServiceProxy.ServiceFactory(
				settings: serviceFactorySettings);

			return serviceFactory;
		}
	}
}
