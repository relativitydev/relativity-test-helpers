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
	public class FoldersHelperTest
	{
		private IHttpRequestHelper _httpRequestHelper;
		private readonly int _workspaceId = 1234567;
		private Mock<IHttpRequestHelper> _httpRequestHelperMocked;
		private IFoldersHelper Sut;

		[SetUp]
		public void SetUp()
		{
			_httpRequestHelperMocked = new Mock<IHttpRequestHelper>();
			_httpRequestHelper = _httpRequestHelperMocked.Object;
			Sut = new FoldersHelper(_httpRequestHelper);
		}

		[TearDown]
		public void Teardown()
		{
			_httpRequestHelper = null;
			_httpRequestHelperMocked = null;
			Sut = null;
		}

		[Test]
		public void GetFolderName()
		{
			//Setup
			string exampleFolderName = "ExampleFolderName";
			string _responseJson = "{\"FolderName\": \"@exampleFolderName\"}";
			_responseJson = _responseJson.Replace("@exampleFolderName", exampleFolderName);
			_httpRequestHelperMocked.Setup(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>())).Returns(_responseJson);


			//Arrange
			int folderArtifactId = 4433221;

			//Act
			string folderName = Sut.GetFolderName(folderArtifactId, _workspaceId);

			//Assert
			Assert.AreEqual(exampleFolderName, folderName);

			//Verify
			_httpRequestHelperMocked.Verify(x => x.SendPostRequest(It.IsAny<BaseRequestModel>(), It.IsAny<string>()), Times.Exactly(1));
		}
	}
}
