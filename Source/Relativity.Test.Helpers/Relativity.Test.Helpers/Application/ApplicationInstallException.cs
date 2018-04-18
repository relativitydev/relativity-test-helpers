using System;

namespace Relativity.Test.Helpers.Application
{
	public class ApplicationInstallException : Exception
	{
		public ApplicationInstallException(string message) : base(message) { }
	}
}
