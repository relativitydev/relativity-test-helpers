using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace Relativity.Test.Helpers.Mail
{
	public class GmailMailHelper : IMailHelper
	{
		private const string Domain = "imap.gmail.com";
		private const int Port = 993;
		private const bool UseSsl = true;

		public string GmailEmail { get; set; }
		public string GmailPassword { get; set; }

		private readonly MailRepository _gmailRepository;

		/// <summary>
		/// With IMAP setup, you should pass in the email and password to this helper to allow it access to your account.
		/// It is recommended to use a separate fake account for testing.
		/// </summary>
		/// <param name="gmailEmail"></param>
		/// <param name="gmailPassword"></param>
		public GmailMailHelper(string gmailEmail, string gmailPassword)
		{
			GmailEmail = gmailEmail;
			GmailPassword = gmailPassword;

			_gmailRepository = new MailRepository(Domain, Port, UseSsl, GmailEmail, GmailPassword);
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
		/// <param name="inbox">Inbox is the user's email address</param>
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
		/// <param name="inbox">Inbox is the user's email address</param>
		/// <param name="messageId">MailTrapMessageModel.id is the source of this</param>
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

		/// <summary>
		/// Deletes the selected message
		/// </summary>
		/// <param name="inbox">/// <param name="inbox">Inbox is the user's email address</param></param>
		/// <param name="messageId">MailTrapMessageModel.id is the source of this</param>
		/// <returns></returns>
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