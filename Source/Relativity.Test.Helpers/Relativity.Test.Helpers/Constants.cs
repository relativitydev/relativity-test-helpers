using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers
{
	public class Constants
	{
		public class Kepler
		{
			public class RouteNames
			{
				public const string GetGuidAsync = "GetGuidAsync";
				public const string GetFieldArtifactIdAsync = "GetFieldArtifactIdAsync";
				public const string GetFieldCountAsync = "GetFieldCountAsync";
				public const string GetFolderNameAsync = "GetFolderNameAsync";
				public const string GetDocumentIdentifierFieldColumnNameAsync = "GetDocumentIdentifierFieldColumnNameAsync";
				public const string GetDocumentIdentifierFieldNameAsync = "GetDocumentIdentifierFieldNameAsync";
			}

			public class KeplerTestRap
			{
				public const string KEPLER_TEST_APP_GUID = "151e017c-fcaa-49e6-ae20-3c2b5e721711";
				public const string KEPLER_TEST_APP_NAME = "TestHelpers_Kepler_App";
			}

			public const string SERVICES_DLL_NAME = "TestHelpersKepler.Services.dll";
			public const string INTERFACES_DLL_NAME = "TestHelpersKepler.Interfaces.dll";
			public const string MINIMUM_KEPLER_COMPATIBLE_VERSION = "10.3.170.1";
			public const int TWENTY_SECONDS = 20000;
		}
	}
}
