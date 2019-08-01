namespace Relativity.Test.Helpers.Mail
{
	public interface IMailMessageModel
	{
		int Id { get; set; }
		int InboxId { get; set; }
		string Message { get; set; }
	}
}
