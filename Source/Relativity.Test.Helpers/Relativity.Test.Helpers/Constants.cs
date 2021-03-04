namespace Relativity.Test.Helpers
{
	public class Constants
	{
		public class ArtifactTypeIds
		{
			public const int System = 1;
			public const int User = 2;
			public const int Group = 3;
			public const int View = 4;
			public const int Client = 5;
			public const int Matter = 6;
			public const int Choice = 7;
			public const int Code = 7;
			public const int Workspace = 8;
			public const int Document = 10;
			public const int Field = 14;
			public const int Layout = 16;
			public const int Error = 18;
			public const int Agent = 20;
			public const int Tab = 23;
			public const int ObjectType = 25;
			public const int RelativityScript = 28;
			public const int SearchIndex = 29;
			public const int ResourceFile = 30;
			public const int ResourcePool = 31;
			public const int ResourceServer = 32;
			public const int ObjectRule = 33;
			public const int LibraryApplication = 34;
			public const int AgentType = 35;
			public const int CaseApplication = 36;
			public const int ApplicationInstall = 37;
			public const int ApplicationInstallResult = 38;
			public const int License = 39;
			public const int InstanceSetting = 42;
			public const int Credential = 43;
		}

		public class TestHelperTemporaryValues
		{
			public class UserHelperTemporaryValues
			{
				public const string FirstName = "Temp User";
				public const string Client = "Relativity Template";
				public const string UserType = "Internal";
				public const string EmailPrefix = "tempuser.";
				public const string EmailSuffix = "@test.com";
				public const string AuthProviderType = "Password";
			}
		}

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
			public const string MINIMUM_KEPLER_COMPATIBLE_VERSION = "10.3.191.8";
			public const int TWENTY_SECONDS = 20000;
		}
	}
}
