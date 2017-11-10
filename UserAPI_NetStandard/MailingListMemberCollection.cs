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
    /// The MailingListMemberCollection object represents mailing list members in mailing list.
    /// </summary>
    public class MailingListMemberCollection : IEnumerable
    {
        private MailingList  m_pMailingList = null;
        private List<string> m_pMembers     = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailingList">Owner mailing list.</param>
        internal MailingListMemberCollection(MailingList mailingList)
        {
            m_pMailingList = mailingList;
            m_pMembers     = new List<string>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds specified member to mailing list.
        /// </summary>
        /// <param name="member">Member to add.</param>
        public void Add(string member)
        {
            /* AddMailingListMember <virtualServerID> "<mailingListID>" "<member>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListMember
            m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddMailingListMember " + m_pMailingList.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pMailingList.ID) + " " + TextUtils.QuoteString(member));
                        
            string response = m_pMailingList.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pMembers.Add(member);
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified member from mebers collection.
        /// </summary>
        /// <param name="member">Member to delete.</param>
        public void Remove(string member)
        {
            /* DeleteMailingListMember <virtualServerID> "<mailingListID>" "<member>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingListMember
            m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteMailingListMember " + m_pMailingList.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pMailingList.ID) + " " + TextUtils.QuoteString(member));
                        
            string response = m_pMailingList.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pMembers.Remove(member);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server mailing list members and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetMailingListMembers <virtualServerID> <mailingListID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pMailingList.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetMailingListMembers
                m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetMailingListMembers " + m_pMailingList.VirtualServer.VirtualServerID + " " + m_pMailingList.ID);

                string response = m_pMailingList.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pMailingList.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("MailingListAddresses")){
                    if(ds.Tables.Contains("MailingListAddresses")){
                        foreach(DataRow dr in ds.Tables["MailingListAddresses"].Rows){
                            m_pMembers.Add(dr["Address"].ToString());
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
			return m_pMembers.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets number of members on that mailing list.
        /// </summary>
        public int Count
        {
            get{ return m_pMembers.Count; }
        }
        
        /// <summary>
        /// Gets a mailing list member in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the mailing list member in the MailingListMemberCollection collection.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get{ return m_pMembers[index]; }
        }

        /// <summary>
        /// Gets a mailing list member in the collection by mailing list member name.
        /// </summary>
        /// <param name="member">A String value that specifies the mailing list member name in the MailingListMemberCollection collection.</param>
        /// <returns></returns>
        public string this[string member]
        {
            get{  
                foreach(string m in m_pMembers){
                    if(m.ToLower() == member.ToLower()){
                        return m;
                    }
                }

                throw new Exception("Mailing list member '" + member + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
