namespace Relativity.Test.Helpers.Mail
{
	public class GmailInboxModel : IMailInboxModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}