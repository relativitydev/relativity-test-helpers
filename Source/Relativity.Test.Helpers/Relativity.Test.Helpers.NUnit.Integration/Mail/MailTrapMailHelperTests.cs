using NUnit.Framework;
using Relativity.Test.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Relativity.Test.Helpers.NUnit.Integration.Mail
{
	public class MailTrapMailHelperTests
	{
		private IMailHelper Sut;

		/// <summary>
		/// You will need to have a MailTrap account setup and will need set ApiKey to the key given to you by the site.
		/// https://mailtrap.io/signin
		/// Also this test assumes you have an email already in the MailTrap Inbox
		/// </summary>
		private const string EmailTestSubject = "Relativity Integration Test";
		private const string EmailTestBody = "Relativity test email for integration tests";
		private const string EmailTestDisplayName = "Relativity ODA";
		private const int EmailPort = 2525;
		private const string EmailDomain = "smtp.mailtrap.io";

		// The following below should be changed for this test
		//private const string ApiKey = "<YOUR_API_KEY>";
		//private const string EmailAddress = "<YOUR_TARGET_EMAIL>"; // Can really be any address since MailTrap will capture it
		//private const string EmailUsername = "<YOUR_USERNAME>";
		//private const string EmailPassword = "<YOUR_PASSWORD>";
		private readonly string ApiKey = TestContext.Parameters["MailTrapTest_ApiKey"];
		private readonly string EmailAddress = TestContext.Parameters["MailTrapTest_EmailAddress"]; // Can really be any address since MailTrap will capture it
		private readonly string EmailUsername = TestContext.Parameters["MailTrapTest_EmailUsername"];
		private readonly string EmailPassword = TestContext.Parameters["MailTrapTest_EmailPassword"];
		private const int SleepTimerInSeconds = 5; //Sleep because MailTraip limits actions per second

		[OneTimeSetUp]
		public void SetUp()
		{
			Sut = new MailTrapMailHelper(ApiKey);
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
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));

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
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			List<IMailInboxModel> inboxes = Sut.GetInboxes();

			// Act 
			List<IMailMessageModel> messages = Sut.GetMessagesInInbox(inboxes.First());

			// Assert
			Assert.NotNull(inboxes.First());
			Assert.NotNull(messages.First());
			Assert.Greater(messages.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(messages.First().Id));

			Sut.DeleteMessage(inboxes.First(), messages.First().Id);
		}

		[Test]
		public void GetMessage()
		{
			// Arrange
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			string textToFind = EmailTestBody.ToLower();


			List<IMailInboxModel> inboxes = Sut.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

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

			Sut.DeleteMessage(inbox, messageId);
		}

		[Test]
		public void DeleteMessage()
		{
			// Arrange
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			List<IMailInboxModel> inboxes = Sut.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			List<IMailMessageModel> messages = Sut.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = Sut.DeleteMessage(inbox, messageId);

			// Assert
			Assert.NotNull(message.Id);
			Assert.IsTrue(message.Id.Equals(messageId, StringComparison.OrdinalIgnoreCase));
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
				Credentials = new NetworkCredential(EmailUsername, EmailPassword)
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