using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models
{
	public class GetDocumentIdentifierFieldColumnNameRequestModel : BaseRequestModel
	{
		public int FieldArtifactTypeId { get; set; }
	}
}
