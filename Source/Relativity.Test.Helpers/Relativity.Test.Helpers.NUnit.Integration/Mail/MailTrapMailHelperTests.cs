using NUnit.Framework;
using Relativity.Test.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

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

			// Act 
			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inboxes.First());

			// Assert
			Assert.Greater(messages.Count, 0);
			Assert.IsFalse(string.IsNullOrEmpty(messages.First().Id));
		}

		[Test]
		public void GetMessage()
		{
			// Arrange
			string textToFind = "Solution Snapshot Job".ToLower();

			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = SuT.GetMessage(inbox, messageId);

			// Assert
			Assert.IsTrue(message.Message.ToLower().Contains(textToFind));
		}

		[Test, Ignore("Ignored to not delete emails that we want to look at")]
		public void DeleteMessage()
		{
			// Arrange
			List<IMailInboxModel> inboxes = SuT.GetInboxes();
			IMailInboxModel inbox = inboxes.First();

			List<IMailMessageModel> messages = SuT.GetMessagesInInbox(inbox);
			string messageId = messages.First().Id;

			// Act 
			IMailMessageModel message = SuT.DeleteMessage(inbox, messageId);

			// Assert
			Assert.IsTrue(message.Id.Equals(messageId, StringComparison.OrdinalIgnoreCase));
		}
	}
}