using NUnit.Framework;
using Relativity.Test.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Relativity.Test.Helpers.NUnit.Integration.Mail
{
	public class GmailMailHelperTests
	{
		private IMailHelper SuT;

		/// <summary>
		/// You will need to have a Gmail account setup with API permissions and oAuth
		/// 
		/// Also this test assumes you have an email already in the MailTrap Inbox
		/// </summary>
		private const string ApiKey = "";
		private const string TestBody = "Relativity test email for integration tests";

		private const string GmailAccount = "";
		private const string GmailPassword = "";
		private const string GmailClientId = "";
		private const string GmailProjectId = "";
		private const string GmailClientSecret = "";
		private const string GmailRedirectUri = "http://localhost";

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new GmailMailHelper(GmailAccount, GmailClientId, GmailClientSecret, GmailProjectId, GmailRedirectUri);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		[Test]
		public void GetInboxes()
		{
			// Arrange

			// Act 
			List<IMailInboxModel> inboxes = SuT.GetInboxes();

			// Assert
			Assert.Greater(inboxes.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(inboxes.First().Id));
		}

		[Test]
		public void GetMessages()
		{
			// Arrange
			List<IMailInboxModel> inboxes = SuT.GetInboxes();

			SendMail();

			// Act 
			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inboxes.First());

			// Assert
			Assert.Greater(messages.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(messages.First().Id));

			//IMailMessageModel message = SuT.DeleteMessage(inboxes.First(), messages.First().Id);
		}

		[Test]
		public void GetMessage()
		{
			// Arrange
			string textToFind = TestBody.ToLower();

			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			SendMail();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message;

			try
			{
				message = SuT.GetMessage(inbox, messageId);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				message = new GmailMessageModel()
				{
					Message = "Failed to parse body"
				};
			}

			// Assert
			Assert.IsTrue(message.Message.ToLower().Contains(textToFind));
			//message = SuT.DeleteMessage(inbox, messageId);
		}

		[Test, Ignore("Ignored for now until Google consents to full API access.  Fun stuff")]
		public void DeleteMessage()
		{
			// Arrange
			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			SendMail();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = SuT.DeleteMessage(inbox, messageId);

			// Assert
			Assert.IsTrue(message.Id == messageId);
		}

		[Test]
		public void SendMail()
		{
			var fromAddress = new MailAddress(GmailAccount, "Relativity ODA");
			var toAddress = new MailAddress(GmailAccount, "Relativity ODA");
			const string subject = "Test Email";
			const string body = "Relativity test email for integration tests";

			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromAddress.Address, GmailPassword)
			};
			using (var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = body
			})
			{
				smtp.Send(message);
			}
		}
	}
}
