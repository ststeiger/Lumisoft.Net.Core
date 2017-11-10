using System;
using System.IO;

using LumiSoft.Net.SMTP.Server;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Specifies filtering result.
	/// </summary>
	public enum FilterResult
	{
		/// <summary>
		/// Store messge and reply Ok to client.
		/// </summary>
		Store = 1,

		/// <summary>
		/// Don't store messge, but reply Ok to client.
		/// </summary>
		DontStore = 2,

		/// <summary>
		/// Send filtering error to client.
		/// </summary>
		Error = 3,
	}

	/// <summary>
	/// SMTP server mail message filter.
	/// </summary>
	public interface ISmtpMessageFilter
	{
		/// <summary>
		/// Filters message.
		/// </summary>
		/// <param name="messageStream">Message stream which to filter.</param>
		/// <param name="filteredStream">Filtered stream.</param>
		/// <param name="sender">Senders email address.</param>
		/// <param name="recipients">Recipients email addresses.</param>
		/// <param name="api">Access to server API.</param>
		/// <param name="session">Reference to SMTP session.</param>
		/// <param name="errorText">Filtering error text what is returned to client. ASCII text, 500 chars maximum.</param>
		FilterResult Filter(Stream messageStream,out Stream filteredStream,string sender,string[] recipients,IMailServerApi api,SMTP_Session session,out string errorText);
	}
}
