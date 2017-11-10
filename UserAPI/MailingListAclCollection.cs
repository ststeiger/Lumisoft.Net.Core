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
    /// The MailingListAclCollection object represents mailing list access list in mailing list.
    /// </summary>
    public class MailingListAclCollection
    {
        private MailingList  m_pMailingList = null;
        private List<string> m_pAcl         = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailingList">Owner mailing list.</param>
        internal MailingListAclCollection(MailingList mailingList)
        {
            m_pMailingList = mailingList;
            m_pAcl         = new List<string>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds specified ACL entry to mailing list.
        /// </summary>
        /// <param name="userOrGroup">User or group.</param>
        public void Add(string userOrGroup)
        {
            /* AddMailingListAcl <virtualServerID> "<mailingListID>" "<userOrGroup>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListAcl
            m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddMailingListAcl " + m_pMailingList.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pMailingList.ID) + " " + TextUtils.QuoteString(userOrGroup));
                        
            string response = m_pMailingList.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pAcl.Add(userOrGroup);
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified ACL entry from mailing list.
        /// </summary>
        /// <param name="userOrGroup">User or group.</param>
        public void Remove(string userOrGroup)
        {
            /* DeleteMailingListAcl <virtualServerID> "<mailingListID>" "<userOrGroup>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListMember
            m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteMailingListAcl " + m_pMailingList.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pMailingList.ID) + " " + TextUtils.QuoteString(userOrGroup));
                        
            string response = m_pMailingList.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pAcl.Remove(userOrGroup);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server mailing list members and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetMailingListAcl <virtualServerID> <mailingListID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pMailingList.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetMailingListAcl
                m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetMailingListAcl " + m_pMailingList.VirtualServer.VirtualServerID + " " + m_pMailingList.ID);

                string response = m_pMailingList.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("ACL")){
                    if(ds.Tables.Contains("ACL")){
                        foreach(DataRow dr in ds.Tables["ACL"].Rows){
                            m_pAcl.Add(dr["UserOrGroup"].ToString());
                        }
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
			return m_pAcl.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets number of ACL entries on that mailing list.
        /// </summary>
        public int Count
        {
            get{ return m_pAcl.Count; }
        }
        
        /// <summary>
        /// Gets a ACL entry in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the ACL entry in the MailingListAclCollection collection.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get{ return m_pAcl[index]; }
        }

        /// <summary>
        /// Gets a ACL entry in the collection by ACL entry name.
        /// </summary>
        /// <param name="aclEntry">A String value that specifies the ACL entry name in the MailingListAclCollection collection.</param>
        /// <returns></returns>
        public string this[string aclEntry]
        {
            get{  
                foreach(string m in m_pAcl){
                    if(m.ToLower() == aclEntry.ToLower()){
                        return m;
                    }
                }

                throw new Exception("Mailing list ACL entry '" + aclEntry + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
