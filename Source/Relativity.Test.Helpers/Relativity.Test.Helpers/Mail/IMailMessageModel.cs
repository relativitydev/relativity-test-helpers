namespace Relativity.Test.Helpers.Mail
{
	public interface IMailMessageModel
	{
		string Id { get; set; }
		string InboxId { get; set; }
		string Message { get; set; }
	}
}
