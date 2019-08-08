namespace Relativity.Test.Helpers.Mail
{
	public interface IMailMessageModel
	{
		string Id { get; set; }
		string InboxId { get; set; }
		string Subject { get; set; }
		string FromEmail { get; set; }
		string ToEmail { get; set; }
		string Body { get; set; }
	}
}
