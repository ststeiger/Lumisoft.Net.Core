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
    /// The GroupCollection object represents user groups in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GroupCollection : IEnumerable
    {
        private VirtualServer m_pVirtualServer = null;
        private List<Group>   m_pGroups        = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal GroupCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pGroups        = new List<Group>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Creates and adds new group to group collection.
        /// </summary>
        /// <param name="name">Group name.</param>
        /// <param name="description">Group description.</param>
        /// <param name="enabled">Specifies if group is enabled.</param>
        /// <returns></returns>
        public Group Add(string name,string description,bool enabled)
        {
            /* AddGroup <virtualServerID> "<groupID>" "<groupName>" "<description>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddGroup " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(id) + " " + TextUtils.QuoteString(name) + " " + TextUtils.QuoteString(description) + " " + enabled);
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            Group group = new Group(m_pVirtualServer,this,id,name,description,enabled);
            m_pGroups.Add(group);
            return group;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes and removes specified group from group collection.
        /// </summary>
        /// <param name="group">Group to delete.</param>
        public void Remove(Group group)
        {
            /* DeleteGroup <virtualServerID> "<groupID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteGroup " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(group.GroupID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pGroups.Remove(group);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains group with specified name.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        public bool Contains(string groupName)
        {
            foreach(Group group in m_pGroups){
                if(group.GroupName.ToLower() == groupName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetGroupByName

        /// <summary>
        /// Gets a Group object in the collection by group name.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns>A Group object value that represents the group in virtual server.</returns>
        public Group GetGroupByName(string groupName)
        {
            foreach(Group group in m_pGroups){
                if(group.GroupName.ToLower() == groupName.ToLower()){
                    return group;
                }
            }

            throw new Exception("Group with specified name '" + groupName + "' doesn't exist !");
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes groups.
        /// </summary>
        public void Refresh()
        {
            m_pGroups.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server groups and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetGroups <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetGroups
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetGroups " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
            
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Groups")){
                    foreach(DataRow dr in ds.Tables["Groups"].Rows){
                        m_pGroups.Add(new Group(
                            m_pVirtualServer,
                            this,
                            dr["GroupID"].ToString(),
                            dr["GroupName"].ToString(),
                            dr["Description"].ToString(),
                            Convert.ToBoolean(dr["Enabled"].ToString())
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
			return m_pGroups.GetEnumerator();
		}

		#endregion


        #region Properties Implementaion

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of groups in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pGroups.Count; }
        }

        /// <summary>
        /// Gets a Group object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Group object in the GroupCollection collection.</param>
        /// <returns>A Group object value that represents the group in virtual server.</returns>
        public Group this[int index]
        {
            get{ return m_pGroups[index]; }
        }

        /// <summary>
        /// Gets a Group object in the collection by group ID.
        /// </summary>
        /// <param name="groupID">A String value that specifies the group ID of the Group object in the GroupCollection collection.</param>
        /// <returns>A Group object value that represents the group in virtual server.</returns>
        public Group this[string groupID]
        {
            get{ 
                foreach(Group group in m_pGroups){
                    if(group.GroupID.ToLower() == groupID.ToLower()){
                        return group;
                    }
                }

                throw new Exception("Group with specified ID '" + groupID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
