using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Windows logon provider.
	/// </summary>
	public class WinLogon
	{
		[DllImport("advapi32.dll", SetLastError=true)]
		private extern static bool LogonUser(string userName,string domainName,string password,int dwLogonType,int dwLogonProvider,ref IntPtr phToken);

        #region static method Logon

        /// <summary>
		/// Logs user to windows.
		/// </summary>
		/// <param name="domain">Windows domain.</param>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		/// <returns>Returns true if logon successful.</returns>
		public static bool Logon(string domain,string userName,string password)
		{
			IntPtr tokenHandle = new IntPtr(0);

			const int LOGON32_PROVIDER_DEFAULT = 0;
			//This parameter causes LogonUser to create a primary token.
			const int LOGON32_LOGON_INTERACTIVE = 2;

			tokenHandle = IntPtr.Zero;

			if(LogonUser(userName,domain,password,LOGON32_LOGON_INTERACTIVE,LOGON32_PROVIDER_DEFAULT,ref tokenHandle)){
				return true;				
			}

			return false;
        }

        #endregion

    }
}
