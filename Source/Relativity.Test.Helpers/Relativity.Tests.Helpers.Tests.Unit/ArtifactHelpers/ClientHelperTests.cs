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

namespace Relativity.Tests.Helpers.Tests.Unit.ArtifactHelpers
{
	[TestFixture]
	public class ClientHelperTests
	{
		private IClientHelper _sut;

		[SetUp]
		public void SetUp()
		{
			_sut = new ClientHelper();
		}
	}
}
