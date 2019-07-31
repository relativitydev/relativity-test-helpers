using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Relativity.Test.Helpers.Mail
{
	public class MailTrapMailHelper : IMailHelper
	{
		public Dictionary<string, string> RequestHeaders { get; set; }
		public string BaseApiUrl => "https://mailtrap.io/";

		public MailTrapMailHelper(string apiKey)
		{
			RequestHeaders = new Dictionary<string, string> { { "Api-Token", apiKey } };
		}

		/// <summary>
		/// Gets all the inboxes on your account
		/// </summary>
		/// <returns></returns>
		public List<IMailInboxModel> GetInboxes()
		{
			List<IMailInboxModel> inboxes = new List<IMailInboxModel>();

			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(BaseApiUrl);

				foreach (var header in RequestHeaders)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				string apiEndpoint = "api/v1/inboxes/";

				HttpResponseMessage response = client.GetAsync(apiEndpoint).Result;
				string json = response.Content.ReadAsStringAsync().Result;

				inboxes.AddRange(JsonConvert.DeserializeObject<List<MailTrapInboxModel>>(json));
			}

			return inboxes;
		}

		/// <summary>
		/// Gets all the messages from a particular inbox in your account
		/// </summary>
		/// <param name="inbox">Used to grab the Id to use in the URL for the API call</param>
		/// <returns></returns>
		public List<IMailMessageModel> GetMessagesInInbox(IMailInboxModel inbox)
		{
			List<IMailMessageModel> messages = new List<IMailMessageModel>();

			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(BaseApiUrl);

				foreach (var header in RequestHeaders)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				string apiEndpoint = $"api/v1/inboxes/{inbox.Id}/messages";

				HttpResponseMessage response = client.GetAsync(apiEndpoint).Result;
				string json = response.Content.ReadAsStringAsync().Result;

				messages.AddRange(JsonConvert.DeserializeObject<List<MailTrapMessageModel>>(json));
			}

			return messages;
		}

		/// <summary>
		/// Returns a specific message as HTML (in string format)
		/// </summary>
		/// <param name="inbox">Used to grab the Id to use in the URL for the API call</param>
		/// <param name="messageId">MailTrapMessageModel.id is the source of this</param>
		/// <returns></returns>
		public IMailMessageModel GetMessage(IMailInboxModel inbox, int messageId)
		{
			IMailMessageModel message;

			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(BaseApiUrl);

				foreach (var header in RequestHeaders)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				string apiEndpoint = $"api/v1/inboxes/{inbox.Id}/messages/{messageId}/body.html";

				HttpResponseMessage response = client.GetAsync(apiEndpoint).Result;

				string messageResult = response.Content.ReadAsStringAsync().Result;

				message = new MailTrapMessageModel()
				{
					Id = messageId,
					InboxId = inbox.Id,
					Message = messageResult
				};
			}

			return message;
		}

		/// <summary>
		/// Simply deletes a message from an inbox, but returns the message before deleting it.
		/// </summary>
		/// <param name="inbox">Used to grab the Id to use in the URL for the API call</param>
		/// <param name="messageId">The ID of the message that is being </param>
		/// <returns></returns>
		public IMailMessageModel DeleteMessage(IMailInboxModel inbox, int messageId)
		{
			MailTrapMessageModel message;

			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(BaseApiUrl);

				foreach (var header in RequestHeaders)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				string apiEndpoint = $"api/v1/inboxes/{inbox.Id}/messages/{messageId}";

				HttpResponseMessage response = client.DeleteAsync(apiEndpoint).Result;
				string json = response.Content.ReadAsStringAsync().Result;

				message = JsonConvert.DeserializeObject<MailTrapMessageModel>(json);
			}

			return message;
		}
	}
}