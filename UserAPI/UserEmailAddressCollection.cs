using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserEmailAddressCollection object represents user email addresses in User.
    /// </summary>
    public class UserEmailAddressCollection : IEnumerable
    {
        private User         m_pUser   = null;
        private List<string> m_pEmails = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="user">Owner user.</param>
        internal UserEmailAddressCollection(User user)
        {
            m_pUser   = user;
            m_pEmails = new List<string>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds specified email address to user.
        /// </summary>
        /// <param name="emailAddress">Email address to add.</param>
        public void Add(string emailAddress)
        {
            /* AddUserEmailAddress <virtualServerID> "<userID>" "<emailAddress>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListMember
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUserEmailAddress " + m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pUser.UserID) + " " + TextUtils.QuoteString(emailAddress));
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pEmails.Add(emailAddress);
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified email address from user.
        /// </summary>
        /// <param name="emailAddress">Email address to remove.</param>
        public void Remove(string emailAddress)
        {
            /* DeleteUserEmailAddress <virtualServerID> "<userID>" "<emailAddress>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListMember
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserEmailAddress " + m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pUser.UserID) + " " + TextUtils.QuoteString(emailAddress));
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pEmails.Remove(emailAddress);
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies collection to array.
        /// </summary>
        /// <returns></returns>
        public string[] ToArray()
        {
            return m_pEmails.ToArray();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server email addresses and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            lock(m_pUser.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserEmailAddresses
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserEmailAddresses " + m_pUser.VirtualServer.VirtualServerID + " " + m_pUser.UserID);

                /* GetUserEmailAddresses <virtualServerID> <userID>
                      Responses:
                        +OK <sizeOfData>
                        <data>
                        
                        + ERR <errorText>
                */
                string response = m_pUser.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("UserAddresses")){
                    foreach(DataRow dr in ds.Tables["UserAddresses"].Rows){
                        m_pEmails.Add(dr["Address"].ToString());
                    }
                }
            }
        }

        #endregion


        #region interface IEnumerator

		/// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pEmails.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
        /*
        /// <summary>
        /// Gets virtual server to where this collection belongs to.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }*/

        /// <summary>
        /// Gets the User object that is the owner of this collection.
        /// </summary>
        public User User
        {
            get{ return m_pUser; }
        }

        /// <summary>
        /// Gets number of email addresses on that user.
        /// </summary>
        public int Count
        {
            get{ return m_pEmails.Count; }
        }
        
        /// <summary>
        /// Gets a user email address in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the user email address in the UserEmailAddressCollection collection.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get{ return m_pEmails[index]; }
        }

        /// <summary>
        /// Gets a user email address in the collection by user email address.
        /// </summary>
        /// <param name="emailAddress">A String value that specifies the user email address in the UserEmailAddressCollection collection.</param>
        /// <returns></returns>
        public string this[string emailAddress]
        {
            get{ 
                foreach(string email in m_pEmails){
                    if(email.ToLower() == emailAddress.ToLower()){
                        return email;
                    }
                }

                throw new Exception("Email address '" + emailAddress + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
