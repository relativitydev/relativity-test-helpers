namespace Relativity.Test.Helpers.Mail
{
	public interface IMailInboxModel
	{
		string Id { get; set; }
		string Name { get; set; }
		string UserName { get; set; }
		string Password { get; set; }
	}
}