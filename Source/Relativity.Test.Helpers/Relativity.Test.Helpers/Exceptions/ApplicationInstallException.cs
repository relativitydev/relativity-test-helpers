using System;

namespace Relativity.Test.Helpers.Exceptions
{
	internal class ApplicationInstallException : Exception
	{
		public ApplicationInstallException(string message) : base(message)
		{
		}
	}
}