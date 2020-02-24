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
		private IMailHelper Sut;

		/// <summary>
		/// You will need to have a Gmail account setup with IMAP
		/// </summary>
		private const string EmailTestSubject = "Relativity Integration Test";
		private const string EmailTestBody = "Relativity test email for integration tests";
		private const string EmailTestDisplayName = "Relativity ODA";
		private const int EmailPort = 587;
		private const string EmailDomain = "smtp.gmail.com";

		// The following below should be changed for this test
		//private const string EmailAddress = "<YOUR_GMAIL_ADDRESS>";
		//private const string EmailPassword = "<YOUR_GMAIL_PASSWORD>";
		private readonly string EmailAddress = TestContext.Parameters["GmailMailTest_EmailAddress"];
		private readonly string EmailPassword = TestContext.Parameters["GmailMailTest_EmailPassword"];

		[OneTimeSetUp]
		public void SetUp()
		{
			Sut = new GmailMailHelper(EmailAddress, EmailPassword);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void GetInboxes()
		{
			// Arrange

			// Act 
			List<IMailInboxModel> inboxes = Sut.GetInboxes();

			// Assert
			Assert.NotNull(inboxes.First());
			Assert.Greater(inboxes.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(inboxes.First().Id));
		}

		[Test]
		public void GetMessages()
		{
			// Arrange
			List<IMailInboxModel> inboxes = Sut.GetInboxes();

			SendMail();

			// Act 
			List<IMailMessageModel> messages = Sut.GetMessagesInInbox(inboxes.First());

			// Assert
			Assert.NotNull(inboxes.First());
			Assert.NotNull(messages.First());
			Assert.Greater(messages.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(messages.First().Id));

			IMailMessageModel message = Sut.DeleteMessage(inboxes.First(), messages.First().Id);
		}

		[Test]
		public void GetMessage()
		{
			// Arrange
			string textToFind = EmailTestBody.ToLower();

			List<IMailInboxModel> inboxes = Sut.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			SendMail();

			List<IMailMessageModel> messages = Sut.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = Sut.GetMessage(inbox, messageId);

			// Assert
			Assert.NotNull(message.Body);
			Assert.IsTrue(message.Body.ToLower().Contains(textToFind));
			Assert.IsTrue(message.Subject.Equals(EmailTestSubject, StringComparison.OrdinalIgnoreCase));
			Assert.IsTrue(message.FromEmail.Contains(EmailAddress));
			Assert.IsTrue(message.ToEmail.Contains(EmailAddress));
			message = Sut.DeleteMessage(inbox, messageId);
		}

		[Test]
		//[Test, Ignore("Ignored for now until Google consents to full API access.  Fun stuff")]
		public void DeleteMessage()
		{
			// Arrange
			List<IMailInboxModel> inboxes = Sut.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			SendMail();

			List<IMailMessageModel> messages = Sut.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = Sut.DeleteMessage(inbox, messageId);

			// Assert
			Assert.NotNull(message.Id);
			Assert.IsTrue(message.Id == messageId);
		}

		public void SendMail()
		{
			MailAddress fromAddress = new MailAddress(EmailAddress, EmailTestDisplayName);
			MailAddress toAddress = new MailAddress(EmailAddress, EmailTestDisplayName);
			const string subject = EmailTestSubject;
			const string body = EmailTestBody;

			SmtpClient smtpClient = new SmtpClient
			{
				Host = EmailDomain,
				Port = EmailPort,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromAddress.Address, EmailPassword)
			};
			using (MailMessage message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = body
			})
			{
				smtpClient.Send(message);
			}
		}
	}
}