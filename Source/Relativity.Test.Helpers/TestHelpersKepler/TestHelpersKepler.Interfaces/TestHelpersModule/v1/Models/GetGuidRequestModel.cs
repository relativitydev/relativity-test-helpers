using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models
{
	public class GetGuidRequestModel : BaseRequestModel
	{
		public int artifactID { get; set; }
		public int workspaceID { get; set; }
	}
}
