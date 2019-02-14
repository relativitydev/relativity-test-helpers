using Relativity.API;

namespace Relativity.Test.Helpers.Logging
{
	public class TestLogFactory : ILogFactory
	{
		private IAPILog _Logger;

		public TestLogFactory(IAPILog loggerToUse)
		{
			this._Logger = loggerToUse;
		}

		public IAPILog GetLogger()
		{
			return _Logger;
		}
	}
}