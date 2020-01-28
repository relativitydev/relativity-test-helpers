using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.ArtifactHelpers.Interfaces
{
	public interface IFieldsHelper
	{
		int GetFieldArtifactId(string fieldname, int workspaceId);
		int GetFieldCount(int artifactId, int workspaceId);

	}
}
