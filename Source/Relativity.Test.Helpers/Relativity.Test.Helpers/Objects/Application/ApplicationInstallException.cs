using System;

namespace Relativity.Test.Helpers.Objects.Application.Exceptions
{
	public class ApplicationInstallException : Exception
	{
		public ApplicationInstallException(string message) : base(message) { }
	}
}
