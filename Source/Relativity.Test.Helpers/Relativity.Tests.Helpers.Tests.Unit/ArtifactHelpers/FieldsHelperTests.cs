using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using Moq;
using NUnit.Framework;
using Relativity.Test.Helpers;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Tests.Helpers.Tests.Unit.ArtifactHelpers
{
	[TestFixture]
	public class FieldsHelperTests
	{
		private IHttpRequestHelper _httpRequestHelper;
		private readonly int _workspaceId = 1234567;
		private Mock<IHttpRequestHelper> _httpRequestHelperMocked;
		private IFieldsHelper Sut;

		[SetUp]
		public void SetUp()
		{
			//_httpRequestHelperMocked = new Mock<IHttpRequestHelper>();
			//_httpRequestHelper = _httpRequestHelperMocked.Object;
			//Sut = new Fields(_httpRequestHelper);
		}

		[TearDown]
		public void Teardown()
		{
			//_httpRequestHelper = null;
			//_httpRequestHelperMocked = null;
			//Sut = null;
		}

		[Test]
		public void GetFieldArtifactId()
		{
			////Setup
			//string _responseJson = "{\"ArtifactId\": 1223344}";
			//_httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>())).Returns(_responseJson);


			////Arrange
			//string fieldName = "TestField";
			//int fieldId = 1223344;

			////act
			//var fieldArtifactId = Sut.GetFieldArtifactId(fieldName, _workspaceId);

			////assert
			//Assert.AreEqual(fieldArtifactId, fieldId);

			////Verify
			//_httpRequestHelperMocked.Verify(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>()), Times.Exactly(1));
		}

		[Test]
		public void GetFieldCount()
		{
			////setup
			//string _responseJson = "{\"Count\": 1}";
			//_httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>())).Returns(_responseJson);

			////arrange
			//int _count = 1;
			//const int fieldArtifactId = 1223344;

			////act
			//var fieldCount = Sut.GetFieldCount(fieldArtifactId, _workspaceId);

			////assert
			//Assert.AreEqual(fieldCount, _count);

			////Verify
			//_httpRequestHelperMocked.Verify(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>()), Times.Exactly(1));
		}
	}
}
