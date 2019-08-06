using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Relativity.Test.Helpers.Mail
{
	public class GmailMailHelper : IMailHelper
	{
		public Dictionary<string, string> RequestHeaders { get; set; }
		public string GmailAccount { get; set; }
		public string GmailClientId { get; set; }
		public string GmailClientSecret { get; set; }
		public string GmailProjectId { get; set; }
		public string GmailRedirectUri { get; set; }
		public string BaseApiUrl => $"https://www.googleapis.com/gmail/v1/users/{GmailAccount}/messages";

		public GmailService GmailService { get; set; }
		static string[] Scopes = { GmailService.Scope.GmailModify };
		static string ApplicationName = "Gmail API .NET Quickstart";

		public GmailMailHelper(string gmailAccount, string gmailClientId, string gmailClientSecret, string gmailProjectId, string gmailRedirectUri)
		{
			GmailAccount = gmailAccount;
			//RequestHeaders = new Dictionary<string, string> { { "Api-Token", apiKey } };
			RequestHeaders = new Dictionary<string, string>();
			GmailClientId = gmailClientId;
			GmailClientSecret = gmailClientSecret;
			GmailProjectId = gmailProjectId;
			GmailRedirectUri = gmailRedirectUri;

			GmailService = CreateGmailService();
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
				Id = GmailAccount
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

			UsersResource.MessagesResource.ListRequest messageRequest = GmailService.Users.Messages.List(inbox.Id);
			messageRequest.MaxResults = 10;
			IList<Message> gmailMessages = messageRequest.Execute().Messages;

			if (gmailMessages != null && gmailMessages.Count > 0)
			{
				foreach (Message gmailMessage in gmailMessages)
				{
					GmailMessageModel message = new GmailMessageModel()
					{
						Id = gmailMessage.Id,
						InboxId = GmailAccount
					};

					messages.Add(message);
				}
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
			GmailMessageModel message = new GmailMessageModel()
			{
				Id = messageId,
				InboxId = inbox.Id
			};

			UsersResource.MessagesResource.GetRequest mailRequest = GmailService.Users.Messages.Get(inbox.Id, message.Id);
			mailRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
			Message email = mailRequest.Execute();

			string from = "";
			string date = "";
			string subject = "";
			string body = "";

			foreach (MessagePartHeader mParts in email.Payload.Headers)
			{
				if (mParts.Name == "Date")
				{
					date = mParts.Value;
				}
				else if (mParts.Name == "From")
				{
					from = mParts.Value;
				}
				else if (mParts.Name == "Subject")
				{
					subject = mParts.Value;
				}

				if (date != "" && from != "")
				{
					if (email.Payload.Parts == null && email.Payload.Body != null)
					{
						body = email.Payload.Body.Data;
					}
					else
					{
						body = GetNestedParts(email.Payload.Parts, "");
					}
					// Need to replace some characters as the data for the email's body is base64
					string codedBody = body.Replace("-", "+");
					codedBody = codedBody.Replace("_", "/");
					try
					{
						byte[] data = Convert.FromBase64String(codedBody);
						body = Encoding.UTF8.GetString(data);

						//Set the returned messages body here
						message.Message = body;
					}
					catch (Exception ex)
					{
						throw new Exception($"Failed to convert email: {message.Id}", ex);
					}
				}
			}

			return message;
		}

		public IMailMessageModel DeleteMessage(IMailInboxModel inbox, string messageId)
		{
			GmailMessageModel message = new GmailMessageModel()
			{
				Id = messageId,
				InboxId = inbox.Id
			};

			UsersResource.MessagesResource.DeleteRequest mailRequest = GmailService.Users.Messages.Delete(inbox.Id, message.Id);
			string result = mailRequest.Execute();

			message.Message = result;

			return message;
		}

		public GmailService CreateGmailService()
		{
			UserCredential credential;

			//using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			using (var stream = GenerateStreamFromGmailCredentials())
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
			}

			// Create Gmail API service.
			GmailService service = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			return service;
		}

		public Stream GenerateStreamFromGmailCredentials()
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			string creds = @"{
	""installed"": {
		""client_id"": ""@@client_id"",
		""project_id"": ""@@project_id"",
		""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
		""token_uri"": ""https://oauth2.googleapis.com/token"",
		""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
		""client_secret"": ""@@client_secret"",
		""redirect_uris"": [ ""urn:ietf:wg:oauth:2.0:oob"", ""@@redirect_uri"" ]
	}
}";
			creds = creds.Replace("@@client_id", GmailClientId);
			creds = creds.Replace("@@project_id", GmailProjectId);
			creds = creds.Replace("@@client_secret", GmailClientSecret);
			creds = creds.Replace("@@redirect_uri", GmailRedirectUri);

			writer.Write(creds);
			writer.Flush();
			stream.Position = 0;

			return stream;
		}

		private String GetNestedParts(IList<MessagePart> part, string curr)
		{
			string str = curr;
			if (part == null)
			{
				return str;
			}
			else
			{
				foreach (MessagePart parts in part)
				{
					if (parts.Parts == null)
					{
						if (parts.Body != null && parts.Body.Data != null)
						{
							str += parts.Body.Data;
						}
					}
					else
					{
						return GetNestedParts(parts.Parts, str);
					}
				}

				return str;
			}
		}

	}
}
