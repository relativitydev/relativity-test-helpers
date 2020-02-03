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

			public const string DEFAULT_APP_GUID = "3E86B18F-8B55-45C4-9A57-9E0CBD7BAF46";
			public const string SERVICES_DLL_NAME = "TestHelpersKepler.Services.dll";
			public const string INTERFACES_DLL_NAME = "TestHelpersKepler.Interfaces.dll";
			public const string KEPLER_APP_NAME = "TestHelpers_Kepler_App";
		}
	}
}
