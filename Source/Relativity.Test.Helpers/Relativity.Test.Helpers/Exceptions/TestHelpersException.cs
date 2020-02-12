using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Exceptions
{
	public class TestHelpersException : System.Exception
	{
		public TestHelpersException()
		{
		}

		public TestHelpersException(string message)
			: base(message)
		{
		}

		public TestHelpersException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client. 
		protected TestHelpersException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
