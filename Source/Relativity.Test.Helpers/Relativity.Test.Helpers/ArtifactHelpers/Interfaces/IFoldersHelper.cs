using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace Relativity.Test.Helpers.ArtifactHelpers.Interfaces
{
	public interface IFoldersHelper
	{
		string GetFolderName(int folderArtifactId, int workspaceId);
	}
}
