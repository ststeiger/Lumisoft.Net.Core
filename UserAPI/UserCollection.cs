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
    /// The UserCollection object represents users in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserCollection : IEnumerable
    {
        private VirtualServer m_pVirtualServer = null;
        private List<User>    m_pUsers         = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal UserCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pUsers         = new List<User>();

            Bind();
        }


        #region method Add

        /// <summary>
		/// Adds new user to virtual server.
		/// </summary>
		/// <param name="userName">User login name.</param>
		/// <param name="fullName">User full name.</param> 
		/// <param name="password">User login password.</param>
		/// <param name="description">User description.</param>
		/// <param name="mailboxSize">Maximum mailbox size in MB.</param>
		/// <param name="enabled">Sepcifies if user is enabled.</param>
		/// <param name="permissions">Specifies user permissions.</param>
        public User Add(string userName,string fullName,string password,string description,int mailboxSize,bool enabled,UserPermissions_enum permissions)
        {
            /* AddUser <virtualServerID> "<userID>" "<userName>" "<fullName>" "<password>" "<description>" <mailboxSize> <enabled> <permissions>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();
            
            // Call TCP AddUser
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUser " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                TextUtils.QuoteString(userName) + " " + 
                TextUtils.QuoteString(fullName) + " " + 
                TextUtils.QuoteString(password) + " " + 
                TextUtils.QuoteString(description) + " " + 
                mailboxSize + " " +
                enabled + " " +
                (int)permissions
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            User user = new User(m_pVirtualServer,this,id,enabled,userName,password,fullName,description,mailboxSize,permissions,DateTime.Now);
            m_pUsers.Add(user);
            return user;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified user from virtual server.
        /// </summary>
        /// <param name="user">User to remove.</param>
        public void Remove(User user)
        {
            /* DeleteUser <virtualServerID> "<userID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUser
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUser " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(user.UserID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pUsers.Remove(user);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains user with specified name.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns></returns>
        public bool Contains(string userName)
        {
            foreach(User user in m_pUsers){
                if(user.UserName.ToLower() == userName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetUserByName

        /// <summary>
        /// Gets a User object in the collection by user name.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>A User object value that represents the user in virtual server.</returns>
        public User GetUserByName(string userName)
        {
            foreach(User user in m_pUsers){
                if(user.UserName.ToLower() == userName.ToLower()){
                    return user;
                }
            }

            throw new Exception("User with specified User Name '" + userName + "' doesn't exist !");
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes users.
        /// </summary>
        public void Refresh()
        {
            m_pUsers.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server users and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetUsers <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */
    
            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetUsers
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUsers " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Users")){
                    foreach(DataRow dr in ds.Tables["Users"].Rows){
                        m_pUsers.Add(new User(
                            m_pVirtualServer,
                            this,
                            dr["UserID"].ToString(),
                            Convert.ToBoolean(dr["Enabled"]),
                            dr["UserName"].ToString(),
                            dr["Password"].ToString(),
                            dr["FullName"].ToString(),
                            dr["Description"].ToString(),
                            Convert.ToInt32(dr["Mailbox_Size"]),
                            (UserPermissions_enum)Convert.ToInt32(dr["Permissions"]),
                            Convert.ToDateTime(dr["CreationTime"])
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
			return m_pUsers.GetEnumerator();
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
        /// Gets number of users in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pUsers.Count; }
        }

        /// <summary>
        /// Gets a User object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the User object in the UserCollection collection.</param>
        /// <returns>A User object value that represents the user in virtual server.</returns>
        public User this[int index]
        {
            get{ return m_pUsers[index]; }
        }

        /// <summary>
        /// Gets a User object in the collection by user ID.
        /// </summary>
        /// <param name="userID">A String value that specifies the user ID of the User object in the UserCollection collection.</param>
        /// <returns>A User object value that represents the user in virtual server.</returns>
        public User this[string userID]
        {
            get{ 
                foreach(User user in m_pUsers){
                    if(user.UserID.ToLower() == userID.ToLower()){
                        return user;
                    }
                }

                throw new Exception("User with specified ID '" + userID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
