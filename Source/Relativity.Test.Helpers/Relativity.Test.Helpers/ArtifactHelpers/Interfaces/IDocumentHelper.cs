using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.ArtifactHelpers.Interfaces
{
	public interface IDocumentHelper
	{
		string GetDocumentIdentifierFieldColumnName(int fieldArtifactTypeID, int workspaceID);
		string GetDocumentIdentifierFieldName(int fieldArtifactTypeID, int workspaceID);
	}
}
