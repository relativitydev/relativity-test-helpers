namespace Relativity.Test.Helpers.Mail
{
	public class GmailMessageModel : IMailMessageModel
	{
		public string Id { get; set; }
		public string InboxId { get; set; }
		public string Subject { get; set; }
		public string FromEmail { get; set; }
		public string ToEmail { get; set; }
		public string Body { get; set; }

	}
}
