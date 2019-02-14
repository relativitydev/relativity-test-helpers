using System;

namespace Relativity.Test.Helpers.Objects.Application.Exceptions
{
	internal class ApplicationInstallException : Exception
	{
		public ApplicationInstallException(string message) : base(message)
		{
		}
	}
}