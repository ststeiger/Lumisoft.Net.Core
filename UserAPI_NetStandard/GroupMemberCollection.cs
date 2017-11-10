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
    /// The GroupMemberCollection object represents group members in group.
    /// </summary>
    public class GroupMemberCollection : IEnumerable
    {
        private Group        m_pGroup   = null;
        private List<string> m_pMembers = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="group">Owner group.</param>
        internal GroupMemberCollection(Group group)
        {
            m_pGroup   = group;
            m_pMembers = new List<string>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds specified user or group to members collection.
        /// </summary>
        /// <param name="userOrGroup">User or group to add.</param>
        public void Add(string userOrGroup)
        {
            /* AddGroupMember <virtualServerID> "<groupID>" "<member>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddGroupMember
            m_pGroup.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddGroupMember " + m_pGroup.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pGroup.GroupID) + " " + TextUtils.QuoteString(userOrGroup));
                        
            string response = m_pGroup.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pMembers.Add(userOrGroup);
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified user or group from mebers collection.
        /// </summary>
        /// <param name="userOrGroup">User or group to delete.</param>
        public void Remove(string userOrGroup)
        {
            /* DeleteGroupMember <virtualServerID> "<groupID>" "<member>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGroupMember
            m_pGroup.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteGroupMember " + m_pGroup.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pGroup.GroupID) + " " + TextUtils.QuoteString(userOrGroup));
                        
            string response = m_pGroup.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pMembers.Remove(userOrGroup);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains group member with specified name.
        /// </summary>
        /// <param name="member">Member name.</param>
        /// <returns></returns>
        public bool Contains(string member)
        {
            foreach(string m in m_pMembers){
                if(m.ToLower() == member.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server group members and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetGroupMembers <virtualServerID> <groupID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pGroup.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetGroupMembers
                m_pGroup.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetGroupMembers " + m_pGroup.VirtualServer.VirtualServerID + " " + m_pGroup.GroupID);

                string response = m_pGroup.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pGroup.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("Members")){
                    foreach(DataRow dr in ds.Tables["Members"].Rows){
                        m_pMembers.Add(dr["Member"].ToString());
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
        /// Gets number of members on that group.
        /// </summary>
        public int Count
        {
            get{ return m_pMembers.Count; }
        }
        
        /// <summary>
        /// Gets a group member in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the group member in the GroupMemberCollection collection.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get{ return m_pMembers[index]; }
        }

        /// <summary>
        /// Gets a group member in the collection by group member name.
        /// </summary>
        /// <param name="member">A String value that specifies the group member name in the GroupMemberCollection collection.</param>
        /// <returns></returns>
        public string this[string member]
        {
            get{  
                foreach(string m in m_pMembers){
                    if(m.ToLower() == member.ToLower()){
                        return m;
                    }
                }

                throw new Exception("Member '" + member + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
