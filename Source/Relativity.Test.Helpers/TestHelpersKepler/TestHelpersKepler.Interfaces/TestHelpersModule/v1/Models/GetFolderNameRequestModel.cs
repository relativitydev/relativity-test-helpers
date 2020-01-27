using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models
{
	public class GetFolderNameRequestModel
	{
		public int FolderArtifactId { get; set; }
		public int WorkspaceId { get; set; }
	}
}
