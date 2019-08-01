using Newtonsoft.Json;

namespace Relativity.Test.Helpers.Mail
{
	public class MailTrapInboxModel : IMailInboxModel
	{
		public int Id { get; set; }
		[JsonProperty("company_id")]
		public int CompanyId { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		[JsonProperty("max_size")]
		public int MaxSize { get; set; }
		public string Status { get; set; }
		[JsonProperty("email_username")]
		public string EmailUserName { get; set; }
		[JsonProperty("email_username_enabled")]
		public bool EmailUserNameEnabled { get; set; }
		[JsonProperty("sent_messages_count")]
		public int SentMessagesCount { get; set; }
		[JsonProperty("forwarded_messages_count")]
		public int ForwardedMessagesCount { get; set; }
		public string Domain { get; set; }
		[JsonProperty("pop3_domain")]
		public string Pop3Domain { get; set; }
		[JsonProperty("email_domain")]
		public string EmailDomain { get; set; }
		[JsonProperty("emails_count")]
		public int EmailsCount { get; set; }
		[JsonProperty("emails_unread_count")]
		public int EmailsUnreadCount { get; set; }
		[JsonProperty("last_message_sent_at_timestamp")]
		public int LastMessageSentAtTimestamp { get; set; }
		[JsonProperty("smtp_ports")]
		public int[] SmtpPorts { get; set; }
		[JsonProperty("pop3_ports")]
		public int[] Pop3Ports { get; set; }
		[JsonProperty("max_message_size")]
		public int MaxMessageSize { get; set; }
		[JsonProperty("has_inbox_address")]
		public bool HasInboxAddress { get; set; }
	}
}