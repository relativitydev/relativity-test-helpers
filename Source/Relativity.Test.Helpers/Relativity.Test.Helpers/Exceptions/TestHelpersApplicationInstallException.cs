namespace Relativity.Test.Helpers.Exceptions
{
	public class TestHelpersApplicationInstallException : System.Exception
	{
		public TestHelpersApplicationInstallException()
		{
		}

		public TestHelpersApplicationInstallException(string message)
			: base(message)
		{
		}

		public TestHelpersApplicationInstallException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client. 
		protected TestHelpersApplicationInstallException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
