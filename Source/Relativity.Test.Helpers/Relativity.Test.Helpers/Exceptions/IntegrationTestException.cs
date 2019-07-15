using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Exceptions
{
	public class IntegrationTestException : System.Exception
	{

		public IntegrationTestException()
		{
		}

		public IntegrationTestException(string message)
			: base(message)
		{
		}

		public IntegrationTestException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client. 
		protected IntegrationTestException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
