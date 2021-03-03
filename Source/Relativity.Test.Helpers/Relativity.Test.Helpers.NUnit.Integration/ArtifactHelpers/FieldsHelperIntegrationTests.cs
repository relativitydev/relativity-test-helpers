using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using FieldType = Relativity.Services.Interfaces.Field.Models.FieldType;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
    [TestFixture]
    public class FieldsHelperIntegrationTests
    {
        private IHelper _testHelper;
        private IServicesMgr _servicesMgr;
        private IDBContext _dbContext;

        private int _workspaceId;
        private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";

        private const string _testFieldName = "IntegrationTestFieldName";
        private int _testFieldId;
        private int _testFieldCount = 1;

        private KeplerHelper _keplerHelper;
        private bool useDbContext;

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
            _servicesMgr = _testHelper.GetServicesManager();

            //Create workspace
            _workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesMgr, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();

            //Query for field ID to be used in test
            _testFieldId = CreateTestField(_servicesMgr, _testFieldName, _workspaceId);

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
            DeleteTestField(_servicesMgr, _testFieldId, _workspaceId);

            //Delete workspace
            Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesMgr, _workspaceId);

            _servicesMgr = null;
            _dbContext = null;
            _testHelper = null;
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
            var fieldId = FieldHelper.CreateFieldDate(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.Date);
        }

        [Test]
        public void CreateFieldUserTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldUser(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.User);
        }

        [Test]
        public void CreateFieldFixedLengthTextTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldFixedLengthText(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.FixedLength);
        }

        [Test]
        public void CreateFieldLongTextTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldLongText(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.LongText);

        }

        [Test]
        public void CreateFieldWholeNumberTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldWholeNumber(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.WholeNumber);
        }

        [Test]
        public void CreateFieldYesNoTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldYesNo(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.YesNo);
        }

        [Test]
        public void CreateFieldSingleChoiceTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldSingleChoice(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.SingleChoice);
        }

        [Test]
        public void CreateFieldMultipleChoiceTest()
        {
            //Act
            var fieldId = FieldHelper.CreateFieldMultipleChoice(_servicesMgr, _workspaceId);

            //Assert
            var createdFieldType = ReadField(fieldId, _workspaceId);
            Assert.IsTrue(createdFieldType == FieldType.MultipleChoice);
        }

        private FieldType ReadField(int fieldId, int workspaceId)
        {
            using (var fieldManager = _servicesMgr.GetProxy<Services.Interfaces.Field.IFieldManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
            {
                var response = fieldManager.ReadAsync(workspaceId, fieldId).ConfigureAwait(false).GetAwaiter().GetResult();
                var fieldType = response.FieldType;
                return fieldType;
            }
        }

        //helper methods

        public int CreateTestField(IServicesMgr servicesMgr, string fieldName, int workspaceId)
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

                using (Services.Interfaces.Field.IFieldManager fieldManager = servicesMgr.CreateProxy<Services.Interfaces.Field.IFieldManager>(ExecutionIdentity.CurrentUser))
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

        public void DeleteTestField(IServicesMgr servicesMgr, int fieldId, int workspaceId)
        {
            try
            {
                using (Services.Interfaces.Field.IFieldManager fieldManager = servicesMgr.CreateProxy<Services.Interfaces.Field.IFieldManager>(ExecutionIdentity.CurrentUser))
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
