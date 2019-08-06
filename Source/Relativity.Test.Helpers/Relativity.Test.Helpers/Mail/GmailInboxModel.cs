namespace Relativity.Test.Helpers.Mail
{
	public class GmailInboxModel : IMailInboxModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		public Message[] Messages { get; set; }
		public int ResultSizeEstimate { get; set; }


		public class Message
		{
			public string Id { get; set; }
			public string ThreadId { get; set; }
		}

	}
}