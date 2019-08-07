using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace Relativity.Test.Helpers.Mail
{
	public class GmailMailHelper : IMailHelper
	{
		public Dictionary<string, string> RequestHeaders { get; set; }
		public string GmailEmail { get; set; }
		public string GmailPassword { get; set; }
		public string BaseApiUrl => $"https://www.googleapis.com/gmail/v1/users/{GmailEmail}/messages";
		private readonly MailRepository _gmailRepository;

		public GmailMailHelper(string gmailEmail, string gmailPassword)
		{
			RequestHeaders = new Dictionary<string, string>();

			GmailEmail = gmailEmail;
			GmailPassword = gmailPassword;

			_gmailRepository = new MailRepository("imap.gmail.com", 993, true, GmailEmail, GmailPassword);
		}

		/// <summary>
		/// Gmail only has 1 Inbox, and the ID is the userId
		/// </summary>
		/// <returns></returns>
		public List<IMailInboxModel> GetInboxes()
		{
			List<IMailInboxModel> inboxes = new List<IMailInboxModel>();
			IMailInboxModel inbox = new GmailInboxModel()
			{
				Id = GmailEmail
			};

			inboxes.Add(inbox);

			return inboxes;
		}

		/// <summary>
		/// Returns a small subset of the latest emails (only their ids) in the inbox
		/// </summary>
		/// <param name="inbox"></param>
		/// <returns></returns>
		public List<IMailMessageModel> GetMessagesInInbox(IMailInboxModel inbox)
		{
			List<IMailMessageModel> messages = new List<IMailMessageModel>();

			IEnumerable<MimeMessage> allEmails = _gmailRepository.GetAllMails();

			foreach (MimeMessage email in allEmails)
			{
				GmailMessageModel message = new GmailMessageModel()
				{
					Id = email.MessageId,
					InboxId = inbox.Id,
					Message = (email.HtmlBody ?? email.TextBody)
				};
				messages.Add(message);
			}

			return messages;
		}

		/// <summary>
		/// Returns a message with text inside.
		/// </summary>
		/// <param name="inbox"></param>
		/// <param name="messageId"></param>
		/// <returns></returns>
		public IMailMessageModel GetMessage(IMailInboxModel inbox, string messageId)
		{
			IEnumerable<MimeMessage> allEmails = _gmailRepository.GetAllMails();

			MimeMessage email = allEmails.First(x => x.MessageId.Equals(messageId));

			GmailMessageModel message = new GmailMessageModel()
			{
				Id = email.MessageId,
				InboxId = inbox.Id,
				Message = (email.HtmlBody ?? email.TextBody)
			};

			return message;
		}

		public IMailMessageModel DeleteMessage(IMailInboxModel inbox, string messageId)
		{
			_gmailRepository.DeleteMail(messageId);
			GmailMessageModel message = new GmailMessageModel()
			{
				Id = messageId,
				InboxId = inbox.Id
			};

			return message;
		}
	}
}