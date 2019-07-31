namespace Relativity.Test.Helpers.Mail
{
	public class MailTrapInboxModel : IMailInboxModel
	{
		public int Id { get; set; }
		public int company_id { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public int max_size { get; set; }
		public string status { get; set; }
		public string email_username { get; set; }
		public bool email_username_enabled { get; set; }
		public int sent_messages_count { get; set; }
		public int forwarded_messages_count { get; set; }
		public string domain { get; set; }
		public string pop3_domain { get; set; }
		public string email_domain { get; set; }
		public int emails_count { get; set; }
		public int emails_unread_count { get; set; }
		public int last_message_sent_at_timestamp { get; set; }
		public int[] smtp_ports { get; set; }
		public int[] pop3_ports { get; set; }
		public int max_message_size { get; set; }
		public bool has_inbox_address { get; set; }
	}
}
