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
	public class FieldsTest
	{
		private IHttpRequestHelper _httpRequestHelper;
		private readonly int _workspaceId = 1234567;

		private IFields _fieldsHelper;

		[OneTimeSetUp]
		public void SetUp()
		{

		}

		[OneTimeTearDown]
		public void Teardown()
		{

		}

		[Test]
		public void GetFieldArtifactId()
		{
			//Setup
			string _responseJson = "{\"ArtifactId\": 1223344}";
			var httpRequestHelperMocked = new Mock<IHttpRequestHelper>();
			httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<RequestModel>(), It.IsAny<string>())).Returns(_responseJson);
			_httpRequestHelper = httpRequestHelperMocked.Object;

			//Arrange
			string fieldName = "TestField";
			int fieldId = 1223344;

			_fieldsHelper = new Fields(_httpRequestHelper);

			//act
			var fieldArtifactId = _fieldsHelper.GetFieldArtifactId(fieldName, _workspaceId);

			//assert
			Assert.AreEqual(fieldArtifactId, fieldId);

		}

		[Test]
		public void GetFieldCount()
		{
			//setup
			string _responseJson = "{\"Count\": 1}";
			var httpRequestHelperMocked = new Mock<IHttpRequestHelper>();
			httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<RequestModel>(), It.IsAny<string>())).Returns(_responseJson);
			_httpRequestHelper = httpRequestHelperMocked.Object;

			//arrange
			int _count = 1;
			const int fieldArtifactId = 1223344;

			_fieldsHelper = new Fields(_httpRequestHelper);

			//act
			var fieldCount = _fieldsHelper.GetFieldCount(fieldArtifactId, _workspaceId);

			//assert
			Assert.AreEqual(fieldCount, _count);
		}
	}
}
