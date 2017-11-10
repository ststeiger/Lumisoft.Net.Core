using System;
using LumiSoft.Net.SMTP.Server;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// SMTP server sender filter.
	/// </summary>
	public interface ISmtpSenderFilter
	{
		/// <summary>
		/// Filters sender.
		/// </summary>
		/// <param name="from">Sender.</param>
		/// <param name="api">Reference to server API.</param>
		/// <param name="session">Reference to SMTP session.</param>
		/// <param name="errorText">Filtering error text what is returned to client. ASCII text, 100 chars maximum.</param>
		/// <returns>Returns true if sender is ok or false if rejected.</returns>
		bool Filter(string from,IMailServerApi api,SMTP_Session session,out string errorText);
	}
}
