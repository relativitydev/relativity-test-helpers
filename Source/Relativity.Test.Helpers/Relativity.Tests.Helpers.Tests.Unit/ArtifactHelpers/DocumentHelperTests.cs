using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Relativity.Test.Helpers;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Tests.Helpers.Tests.Unit.ArtifactHelpers
{
	[TestFixture]
	public class DocumentHelperTests
	{
		private IHttpRequestHelper _httpRequestHelper;
		private const int _workspaceId = 1234567;
		private Mock<IHttpRequestHelper> _httpRequestHelperMocked;
		private IDocumentHelper Sut;

		[SetUp]
		public void SetUp()
		{
			//_httpRequestHelperMocked = new Mock<IHttpRequestHelper>();
			//_httpRequestHelper = _httpRequestHelperMocked.Object;
			//Sut = new Document(_httpRequestHelper);
		}

		[TearDown]
		public void Teardown()
		{
			//_httpRequestHelper = null;
			//_httpRequestHelperMocked = null;
			//Sut = null;
		}

		[Test]
		public void GetDocumentIdentifierFieldColumnName()
		{
			////Arrange
			//const string testColumnName = "TestColumnName";
			//const int fieldArtifactId = 1223344;
			//string _responseJson = "{\"ColumnName\": \"@testColumnName\"}";
			//_responseJson = _responseJson.Replace("@testColumnName", testColumnName);
			//_httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>())).Returns(_responseJson);


			////Act
			//string columnName = Sut.GetDocumentIdentifierFieldColumnName(fieldArtifactId, _workspaceId);

			////Assert
			//Assert.AreEqual(testColumnName, columnName);
			//_httpRequestHelperMocked.Verify(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>()), Times.Exactly(1));
		}

		[Test]
		public void GetDocumentIdentifierFieldName()
		{
			////Arrange
			//const string testFieldName = "TestFieldName";
			//const int fieldArtifactId = 1223344;
			//string _responseJson = "{\"FieldName\": \"@testFieldName\"}";
			//_responseJson = _responseJson.Replace("@testFieldName", testFieldName);
			//_httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>())).Returns(_responseJson);


			////Act
			//string columnName = Sut.GetDocumentIdentifierFieldName(fieldArtifactId, _workspaceId);

			////Assert
			//Assert.AreEqual(testFieldName, columnName);
			//_httpRequestHelperMocked.Verify(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>()), Times.Exactly(1));
		}
	}
}
