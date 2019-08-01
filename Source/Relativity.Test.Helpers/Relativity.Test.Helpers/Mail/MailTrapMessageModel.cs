using Newtonsoft.Json;
using System;

namespace Relativity.Test.Helpers.Mail
{
	public class MailTrapMessageModel : IMailMessageModel
	{
		public int Id { get; set; }
		[JsonProperty("inbox_id")]
		public int InboxId { get; set; }
		public string Message { get; set; }
		public string Subject { get; set; }
		[JsonProperty("sent_at")]
		public DateTime SentAt { get; set; }
		[JsonProperty("from_email")]
		public string FromEmail { get; set; }
		[JsonProperty("from_name")]
		public string FromName { get; set; }
		[JsonProperty("to_email")]
		public string ToEmail { get; set; }
		[JsonProperty("to_name")]
		public string ToName { get; set; }
		[JsonProperty("email_size")]
		public int EmailSize { get; set; }
		[JsonProperty("is_read")]
		public bool IsRead { get; set; }
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }
		[JsonProperty("html_body_size")]
		public int HtmlBodySize { get; set; }
		[JsonProperty("text_body_size")]
		public int TextBodySize { get; set; }
		[JsonProperty("sent_at_timestamp")]
		public int SentAtTimestamp { get; set; }
		[JsonProperty("human_size")]
		public string HumanSize { get; set; }
		[JsonProperty("html_path")]
		public string HtmlPath { get; set; }
		[JsonProperty("txt_path")]
		public string TxtPath { get; set; }
		[JsonProperty("raw_path")]
		public string RawPath { get; set; }
		[JsonProperty("download_path")]
		public string DownloadPath { get; set; }
		[JsonProperty("html_source_path")]
		public string HtmlSourcePath { get; set; }
		[JsonProperty("blacklists_report_info")]
		public BlacklistsReportInfo BlacklistsReportInfo { get; set; }
	}

	public class BlacklistsReportInfo
	{
		public string Result { get; set; }
		public string Domain { get; set; }
		public string Ip { get; set; }
		public Report[] Report { get; set; }
	}

	public class Report
	{
		public string Name { get; set; }
		public string Url { get; set; }
		[JsonProperty("in_black_list")]
		public bool InBlackList { get; set; }
	}
}