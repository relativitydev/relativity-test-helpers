using System;

namespace Relativity.Test.Helpers.ServiceFactory
{
	internal class URLHelper : API.IUrlHelper
	{
		public Uri GetApplicationURL(Guid appGuid)
		{
			throw new NotImplementedException();
		}

		public string GetRelativePathToCustomPages(Guid appGuid)
		{
			throw new NotImplementedException();
		}
	}
}