using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.FieldManager;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
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
		private IDBContext _dbContext;

		private int _workspaceId;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		private const string _testFieldName = "IntegrationTestFieldName";
		private int _testFieldId;
		private int _testFieldCount = 1;

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

			_serviceFactory = GetServiceFactory();

			//Create workspace
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).ConfigureAwait(false).GetAwaiter().GetResult();

			//Query for field ID to be used in test
			_testFieldId = CreateTestField(_serviceFactory, _testFieldName, _workspaceId);
			
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
			//delete test field
			DeleteTestField(_serviceFactory, _testFieldId, _workspaceId);

			//Delete workspace
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = null;
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
			Assert.AreEqual(fieldArtifactId, _testFieldId);
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
				fieldCount = FieldHelper.GetFieldCount(_dbContext, _testFieldId);
			}
			else
			{
				fieldCount = FieldHelper.GetFieldCount(_testFieldId, _workspaceId, _keplerHelper);
			}

			//ASSERT
			Assert.AreEqual(fieldCount, _testFieldCount);
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

		//helper methods

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

		public int CreateTestField(Services.ServiceProxy.ServiceFactory serviceFactory, string fieldName, int workspaceId)
		{
			try
			{
				int fieldId;
				var fieldRequest = new WholeNumberFieldRequest()
				{
					Name = fieldName,
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
					Notes = "Created for FieldsHelper Integration Tests"
				};

				using (Services.Interfaces.Field.IFieldManager fieldManager = serviceFactory.CreateProxy<Services.Interfaces.Field.IFieldManager>())
				{
					fieldId = fieldManager.CreateWholeNumberFieldAsync(workspaceId, fieldRequest).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return fieldId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating test field.", ex);
			}
		}

		public void DeleteTestField(Services.ServiceProxy.ServiceFactory serviceFactory, int fieldId, int workspaceId)
		{
			try
			{
				using (Services.Interfaces.Field.IFieldManager fieldManager = serviceFactory.CreateProxy<Services.Interfaces.Field.IFieldManager>())
				{
					fieldManager.DeleteAsync(workspaceId, fieldId).ConfigureAwait(false).GetAwaiter().GetResult();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting test field.", ex);
			}
		}
	}
}
