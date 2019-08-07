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
		private IMailHelper SuT;

		/// <summary>
		/// You will need to have a MailTrap account setup and will need set ApiKey to the key given to you by the site.
		/// https://mailtrap.io/signin
		/// Also this test assumes you have an email already in the MailTrap Inbox
		/// </summary>
		private const string ApiKey = "";
		private const string EmailTestSubject = "Relativity Integration Test";
		private const string EmailTestBody = "Relativity test email for integration tests";
		private const string EmailTestDisplayName = "Relativity ODA";
		private const int EmailPort = 2525;
		private const string EmailDomain = "smtp.mailtrap.io";

		private const string EmailAddress = ""; // Can really be any address since MailTrap will capture it
		private const string EmailUsername = "";
		private const string EmailPassword = "";
		private const int SleepTimerInSeconds = 5; //Sleep because MailTraip limits actions per second

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new MailTrapMailHelper(ApiKey);
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
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));

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
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			List<IMailInboxModel> inboxes = SuT.GetInboxes();

			// Act 
			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inboxes.First());

			// Assert
			Assert.Greater(messages.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(messages.First().Id));

			SuT.DeleteMessage(inboxes.First(), messages.First().Id);
		}

		[Test]
		public void GetMessage()
		{
			// Arrange
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			string textToFind = EmailTestBody.ToLower();


			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = SuT.GetMessage(inbox, messageId);

			// Assert
			Assert.IsTrue(message.Message.ToLower().Contains(textToFind));

			SuT.DeleteMessage(inbox, messageId);
		}

		[Test]
		public void DeleteMessage()
		{
			// Arrange
			Thread.Sleep(TimeSpan.FromSeconds(SleepTimerInSeconds));
			SendMail();
			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = SuT.DeleteMessage(inbox, messageId);

			// Assert
			Assert.IsTrue(message.Id.Equals(messageId, StringComparison.OrdinalIgnoreCase));
		}

		public void SendMail()
		{
			MailAddress fromAddress = new MailAddress(EmailAddress, EmailTestDisplayName);
			MailAddress toAddress = new MailAddress(EmailAddress, EmailTestDisplayName);
			const string subject = EmailTestSubject;
			const string body = EmailTestBody;

			SmtpClient smtp = new SmtpClient
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
				smtp.Send(message);
			}
		}
	}
}