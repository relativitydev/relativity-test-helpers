using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Exceptions
{
	class DbContextHelperException : Exception
	{
		public DbContextHelperException()
		{
		}

		public DbContextHelperException(string message) : base(message)
		{
		}

		public DbContextHelperException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DbContextHelperException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
