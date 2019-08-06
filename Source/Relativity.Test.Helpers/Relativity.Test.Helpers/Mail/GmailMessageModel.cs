namespace Relativity.Test.Helpers.Mail
{
	public class GmailMessageModel : IMailMessageModel
	{
		public string Id { get; set; }
		public string InboxId { get; set; }
		public string Message { get; set; }

		public string ThreadId { get; set; }
		public string[] LabelIds { get; set; }
		public string Snippet { get; set; }
		public string HistoryId { get; set; }
		public string InternalDate { get; set; }
		public Payload payload { get; set; }
		public int SizeEstimate { get; set; }


		public class Payload
		{
			public string PartId { get; set; }
			public string MimeType { get; set; }
			public string Filename { get; set; }
			public Header[] Headers { get; set; }
			public Body Body { get; set; }
			public Part[] Parts { get; set; }
		}

		public class Body
		{
			public int Size { get; set; }
		}

		public class Header
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		public class Part
		{
			public string PartId { get; set; }
			public string MimeType { get; set; }
			public string Filename { get; set; }
			public Header1[] Headers { get; set; }
			public Body1 Body { get; set; }
		}

		public class Body1
		{
			public int Size { get; set; }
			public string Data { get; set; }
		}

		public class Header1
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

	}
}
