using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace Relativity.Test.Helpers.Mail
{
	/// <summary>
	/// Gathered some help from https://stackoverflow.com/questions/7056715/reading-emails-from-gmail-in-c-sharp/19570553#19570553
	/// Gmail API's were getting cumbersome, this was a cleaner solution
	/// </summary>
	public class MailRepository
	{
		private readonly string _mailServer, _login, _password;
		private readonly int _port;
		private readonly bool _ssl;

		public MailRepository(string mailServer, int port, bool ssl, string login, string password)
		{
			_mailServer = mailServer;
			_port = port;
			_ssl = ssl;
			_login = login;
			_password = password;
		}

		public IEnumerable<MimeMessage> GetAllMails()
		{
			List<MimeMessage> messages = new List<MimeMessage>();

			using (ImapClient client = new ImapClient())
			{
				client.Connect(_mailServer, _port, _ssl);

				// Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove("XOAUTH2");

				client.Authenticate(_login, _password);

				// The Inbox folder is always available on all IMAP servers...
				IMailFolder inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadOnly);
				SearchResults results = inbox.Search(SearchOptions.All, SearchQuery.All);
				foreach (UniqueId uniqueId in results.UniqueIds)
				{
					MimeMessage message = inbox.GetMessage(uniqueId);

					messages.Add(message);
				}

				client.Disconnect(true);
			}

			return messages.OrderByDescending(x => x.Date);
		}

		public void DeleteMail(string messageId)
		{
			using (ImapClient client = new ImapClient())
			{
				client.Connect(_mailServer, _port, _ssl);

				// Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove("XOAUTH2");

				client.Authenticate(_login, _password);

				// The Inbox folder is always available on all IMAP servers...
				IMailFolder inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadWrite);
				SearchResults results = inbox.Search(SearchOptions.All, SearchQuery.NotSeen);

				foreach (UniqueId uniqueId in results.UniqueIds)
				{
					MimeMessage message = inbox.GetMessage(uniqueId);

					if (message.MessageId.Equals(messageId))
					{
						inbox.AddFlags(uniqueId, MessageFlags.Deleted, true);
						break;
					}
				}

				client.Disconnect(true);
			}
		}
	}

}