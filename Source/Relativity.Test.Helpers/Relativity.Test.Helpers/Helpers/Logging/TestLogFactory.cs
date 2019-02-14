using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
