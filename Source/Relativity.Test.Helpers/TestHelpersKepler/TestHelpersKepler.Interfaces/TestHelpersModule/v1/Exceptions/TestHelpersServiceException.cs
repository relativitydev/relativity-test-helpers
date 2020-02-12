using System;
using System.Net;
using System.Runtime.Serialization;
using Relativity.Services.Exceptions;

namespace TestHelpersKepler.Interfaces.TestHelpersModule.v1.Exceptions
{
	// Note: Set the ExceptionIdentifier so that exceptions can be identified across namespaces.
	[ExceptionIdentifier("323e2dcb-85cb-4a18-9434-3a3af7f497fc")]
	[FaultCode(HttpStatusCode.NotFound)]
	public class TestHelpersServiceException : ServiceException
	{
		public TestHelpersServiceException()
			: base()
		{ }

		public TestHelpersServiceException(string message)
			: base(message)
		{ }

		public TestHelpersServiceException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		#region Optional FaultSafe information.

		/// <summary>
		/// FaultSafe information is optional.
		/// Additional information such as exception properties and stack traces are only sent when Relativity is set to "Developer mode."
		/// To pass along additional information use the FaultSafe attribute on a property of a custom exception.
		/// Fault safe information must be serializable and will be passed along with the exception at all times.
		/// </summary>
		[FaultSafe]
		public FaultSafeInfo FaultSafeObject { get; set; }

		protected TestHelpersServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				this.FaultSafeObject = (FaultSafeInfo)info.GetValue("FaultSafeObject", typeof(FaultSafeInfo));
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("FaultSafeObject", FaultSafeObject);
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Example FaultSafe serializable object.
		/// </summary>
		[Serializable]
		public class FaultSafeInfo : ISerializable
		{
			/// <summary>
			/// Information sent as a string.
			/// </summary>
			public string Information { get; set; }

			/// <summary>
			/// Additional serializable information to be sent with the FaultSafe Exception.
			/// </summary>
			public DateTime Time { get; set; }

			/// <summary>
			/// Serializable objects must have a default constructor.
			/// </summary>
			public FaultSafeInfo()
			{ }

			public FaultSafeInfo(string information, DateTime time)
			{
				Information = information;
				Time = time;
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("Information", Information);
				info.AddValue("Time", Time);
			}
		}
		#endregion

	}
}
