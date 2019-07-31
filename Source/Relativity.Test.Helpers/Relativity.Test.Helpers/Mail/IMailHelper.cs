﻿using System.Collections.Generic;

namespace Relativity.Test.Helpers.Mail
{
	public interface IMailHelper
	{
		Dictionary<string, string> RequestHeaders { get; set; }
		string BaseApiUrl { get; }
		List<IMailInboxModel> GetInboxes();
		List<IMailMessageModel> GetMessagesInInbox(IMailInboxModel inbox);
		IMailMessageModel GetMessage(IMailInboxModel inbox, int messageId);
		IMailMessageModel DeleteMessage(IMailInboxModel inbox, int messageId);
	}
}