using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;
using Moq;
using NUnit.Framework;
using Relativity.Test.Helpers.Import;
using Relativity.Test.Helpers.Import.Request;

namespace Relativity.Tests.Helpers.Tests.Unit.ImportAPI
{
    [TestFixture]
    public class ImportAPIHelperTest
    {
        const int TEST_IDENTIFIER_FIELD_ID = 1015425;
        const string TEST_IDENTIFIER_FIELD_NAME = "Sample";
        IImportAPI iapi;

        [SetUp]
        public void Setup()
        {
            var iapiMocked = new Mock<IImportAPI>();
            var identifierField = Mock.Of<Field>();
            var fieldType = typeof(Field);
            fieldType.GetProperty("FieldCategory").SetValue(identifierField, FieldCategoryEnum.Identifier);
            fieldType.GetProperty("ArtifactID").SetValue(identifierField, TEST_IDENTIFIER_FIELD_ID);
            fieldType.GetProperty("Name").SetValue(identifierField, TEST_IDENTIFIER_FIELD_NAME);


            iapiMocked.Setup(p => p.GetWorkspaceFields(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new[]
                {
                    identifierField
                });
            iapiMocked.Setup(p => p.NewNativeDocumentImportJob())
                .Returns(Mock.Of<ImportBulkArtifactJob>());
            iapiMocked.Setup(p => p.NewObjectImportJob(It.IsAny<int>()))
                .Returns(Mock.Of<ImportBulkArtifactJob>());

            iapi = iapiMocked.Object;
        }

        [Test]
        public void GetImportJobTest()
        {
            //arrange
            var workspaceID = 1050630;
            var jobRequest = new ImportJobRequest(1050630);

			//act
			      var importJob = new ImportAPIHelper(new Test.Helpers.Configuration.Models.ConfigurationModel()).GetImportJob(jobRequest, iapi);

            //assert
            Assert.AreEqual(workspaceID, importJob.Settings.CaseArtifactId);
            Assert.AreEqual(TEST_IDENTIFIER_FIELD_ID, importJob.Settings.IdentityFieldId);
            Assert.AreEqual(TEST_IDENTIFIER_FIELD_NAME, importJob.Settings.SelectedIdentifierFieldName);
        }


        [Test]
        public void GetDataTableTest()
        {
            //arrange
            var jobRequest = new ImportJobRequest(1050630);
            var importJob = new ImportAPIHelper(new Test.Helpers.Configuration.Models.ConfigurationModel()).GetImportJob(jobRequest, iapi);

            //act
            //var dataTable = importJob.GetDocumentDataTableFromFolder(".");

            //assert
            //Assert.Greater(dataTable.Rows.Count, 0);
        }
    }
}
