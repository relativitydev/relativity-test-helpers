using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using NUnit.Framework;

namespace Relativity.Test.Helpers.NUnit.Integration.WorkspaceHelpers
{
	public class CreateWorkspaceHelperTests
	{
		private IRSAPIClient _client;
		private int _workspaceId;
		private string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		[OneTimeSetUp]
		public void SetUp()
		{

		}

		[OneTimeTearDown]
		public void Teardown()
		{

		}

		[Test]
		public void CreateWorkspaceTest()
		{

		}

		[Test]
		public void CreateWorkspaceTest_Failure()
		{

		}
	}
}
