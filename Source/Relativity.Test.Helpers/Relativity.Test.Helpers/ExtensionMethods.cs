using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using Relativity.Test.Helpers.Exceptions;

namespace Relativity.Test.Helpers
{
	public static class ExtensionMethods
	{
		public static int EnsureSuccess<T>(this ResultSet<T> result) where T : kCura.Relativity.Client.DTOs.Artifact
		{
			int artifactId;

			if (result == null)
			{
				throw new ArgumentNullException(nameof(result));
			}
			else if (!result.Success)
			{
				string message = result.Message;
				if (string.IsNullOrWhiteSpace(message) ||
						(message ?? string.Empty).Contains("see individual results for more details"))
				{
					message += string.Join(",", result.Results.Select(x => x.Message).Where(x => !string.IsNullOrWhiteSpace(x)));
				}

				if (string.IsNullOrWhiteSpace(message))
				{
					message = "An unknown error occurred.";
				}
				throw new TestHelpersException(message);
			}
			else
			{
				if (result.Results.Count == 0)
				{
					throw new TestHelpersException($"Did not return any results [{nameof(result)}:{result}]");
				}
				else if (result.Results.Count > 1)
				{
					throw new TestHelpersException($"Returned more than 1 result [{nameof(result)}:{result}]");
				}
				else
				{
					artifactId = result.Results.FirstOrDefault().Artifact.ArtifactID;
					return artifactId;
				}
			}
		}
	}
}
