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
    /// The MailingListCollection object represents mailing lists in LumiSoft Mail Server virtual server.
    /// </summary>
    public class MailingListCollection : IEnumerable
    {
        private VirtualServer     m_pVirtualServer = null;
        private List<MailingList> m_pMailingLists  = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal MailingListCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pMailingLists  = new List<MailingList>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new mailing list to virtual server.
        /// </summary>
        /// <param name="name">Mailing list name.</param>
        /// <param name="description">Mailing list description.</param>
        /// <param name="enabled">Specifies if mailing list is enabled.</param>
        public MailingList Add(string name,string description,bool enabled)
        {
            /* AddMailingList <virtualServerID> "<mailingListID>" "<mailingListName>" "<description>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddMailingList
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddMailingList " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(id) + " " + TextUtils.QuoteString(name) + " " + TextUtils.QuoteString(description) + " " + enabled);
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            MailingList mailingList = new MailingList(m_pVirtualServer,this,id,name,description,enabled);
            m_pMailingLists.Add(mailingList);
            return mailingList;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified mailing list from virtual server.
        /// </summary>
        /// <param name="mailingList"></param>
        public void Remove(MailingList mailingList)
        {
            /* DeleteMailingList <virtualServerID> "<mailingListID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteMailingList " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(mailingList.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pMailingLists.Remove(mailingList);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains mailing list with specified name.
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        /// <returns></returns>
        public bool Contains(string mailingListName)
        {
            foreach(MailingList list in m_pMailingLists){
                if(list.Name.ToLower() == mailingListName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetMailingListByName

        /// <summary>
        /// Gets a MailingList object in the collection by mailing list name.
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        /// <returns>A MailingList object value that represents the mailing list in virtual server.</returns>
        public MailingList GetMailingListByName(string mailingListName)
        {
            foreach(MailingList list in m_pMailingLists){
                if(list.Name.ToLower() == mailingListName.ToLower()){
                    return list;
                }
            }

            throw new Exception("Mailing list with specified name '" + mailingListName + "' doesn't exist !");
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes mailing lists.
        /// </summary>
        public void Refresh()
        {
            m_pMailingLists.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server mailing lists and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetMailingLists <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetMailingLists
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetMailingLists " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("MailingLists")){
                    foreach(DataRow dr in ds.Tables["MailingLists"].Rows){
                        m_pMailingLists.Add(new MailingList(
                            m_pVirtualServer,
                            this,
                            dr["MailingListID"].ToString(),
                            dr["MailingListName"].ToString(),
                            dr["Description"].ToString(),
                            Convert.ToBoolean(dr["Enabled"])
                        ));
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
			return m_pMailingLists.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of mailing lists in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pMailingLists.Count; }
        }

        /// <summary>
        /// Gets a MailingList object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the MailingList object in the MailingListCollection collection.</param>
        /// <returns>A MailingList object value that represents the mailing list in virtual server.</returns>
        public MailingList this[int index]
        {
            get{ return m_pMailingLists[index]; }
        }

        /// <summary>
        /// Gets a MailingList object in the collection by mailing list ID.
        /// </summary>
        /// <param name="mailingListID">A String value that specifies the mailing list ID of the Group object in the MailingListCollection collection.</param>
        /// <returns>A MailingList object value that represents the mailing list in virtual server.</returns>
        public MailingList this[string mailingListID]
        {
            get{ 
                foreach(MailingList mailingList in m_pMailingLists){
                    if(mailingList.ID.ToLower() == mailingListID.ToLower()){
                        return mailingList;
                    }
                }

                throw new Exception("Mailing list with specified ID '" + mailingListID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
