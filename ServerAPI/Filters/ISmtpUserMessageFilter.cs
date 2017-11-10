using System;
using System.IO;
using LumiSoft.Net.SMTP.Server;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// SMTP server user mail message filter.
	/// </summary>
	public interface ISmtpUserMessageFilter
	{
		/// <summary>
		/// Filters user message.
		/// </summary>
		/// <param name="messageStream">Message stream which to filter.</param>
		/// <param name="filteredStream">Filtered stream.</param>
		/// <param name="userName">User name.</param>
		/// <param name="to">User's to address.</param>
		/// <param name="api">Reference to server API.</param>
		/// <param name="session">Reference to SMTP session.</param>
		/// <param name="storeFolder">Folder to store message.</param>
		/// <param name="errorText">Filtering error text. ASCII text, 100 chars maximum.</param>
		void Filter(MemoryStream messageStream,out MemoryStream filteredStream,string userName,string to,IMailServerApi api,SMTP_Session session,out string storeFolder,out string errorText);
	}
}
