namespace Relativity.Test.Helpers.Exceptions
{
	public class TestHelpersOAuth2Exception : System.Exception
	{
		public TestHelpersOAuth2Exception()
		{
		}

		public TestHelpersOAuth2Exception(string message)
			: base(message)
		{
		}

		public TestHelpersOAuth2Exception(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client. 
		protected TestHelpersOAuth2Exception(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
