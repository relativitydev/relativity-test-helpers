using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.ArtifactHelpers.Interfaces
{
	public interface IClientHelper
	{
		int CreateClient(Services.ServiceProxy.ServiceFactory serviceFactory, string name);
		void DeleteClient(Services.ServiceProxy.ServiceFactory serviceFactory, int artifactId);
	}
}
