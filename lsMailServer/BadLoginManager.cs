using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Bad login manager.
    /// </summary>
    public class BadLoginManager : IDisposable
    {
        #region class BadLoginEntry

        /// <summary>
        /// This class holds bad login entry info.
        /// </summary>
        private class BadLoginEntry
        {
            #region class UserEntry

            /// <summary>
            /// This class holds user bad login info.
            /// </summary>
            private class UserEntry
            {
                private string   m_UserName      = "";
                private DateTime m_CreationTime;
                private int      m_BadLoginCount = 1;

                /// <summary>
                /// Default constructor.
                /// </summary>
                /// <param name="userName">User name.</param>
                public UserEntry(string userName)
                {
                    m_UserName = userName;

                    m_CreationTime = DateTime.Now;
                }


                #region method IncreaseBadLoginCount

                /// <summary>
                /// Increases this entry user bad logins count.
                /// </summary>
                public void IncreaseBadLoginCount()
                {
                    m_BadLoginCount++;
                }

                #endregion


                #region Properties Implementation

                /// <summary>
                /// Gets thios entry related user name.
                /// </summary>
                public string UserName
                {
                    get{ return m_UserName; }
                }

                /// <summary>
                /// Gets time when this entry was created.
                /// </summary>
                public DateTime CreationTime
                {
                    get{ return m_CreationTime; }
                }

                /// <summary>
                /// Gets how many bad logins specified entry user has done. 
                /// </summary>
                public int BadLoginCount
                {
                    get{ return m_BadLoginCount; }
                }

                #endregion

            }

            #endregion

            private string                       m_IP            = "";
            private DateTime                     m_CreationTime;
            private Dictionary<string,UserEntry> m_pUsers        = null;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="ip">IP address.</param>
            public BadLoginEntry(string ip)
            {
                m_IP = ip;

                m_CreationTime = DateTime.Now;
                m_pUsers = new Dictionary<string,UserEntry>();
            }


            #region method IncreaseBadLoginCount

            /// <summary>
            /// Increases specified user bad logins count.
            /// </summary>
            public void IncreaseBadLoginCount(string userName)
            {
                userName = userName.ToLower();

                lock(m_pUsers){
                    if(m_pUsers.ContainsKey(userName)){
                        m_pUsers[userName].IncreaseBadLoginCount();
                    }
                    else{
                        m_pUsers.Add(userName,new UserEntry(userName));
                    }
                }
            }

            #endregion

            #region method GetUserBadLoginCount

            /// <summary>
            /// Gets specified user name bad logins count.
            /// </summary>
            /// <param name="userName">User name.</param>
            /// <returns></returns>
            public int GetUserBadLoginCount(string userName)
            {
                userName = userName.ToLower();

                lock(m_pUsers){
                    if(m_pUsers.ContainsKey(userName)){
                        return m_pUsers[userName].BadLoginCount;
                    }
                    else{
                        return 0;
                    }
                }
            }

            #endregion

            #region method RemoveOlderThan

            /// <summary>
            /// Removes older than specified seconds entries from collection.
            /// </summary>
            /// <param name="seconds">Time in seconds.</param>
            public void RemoveOlderThan(int seconds)
            {
                List<string> usersToRemove = new List<string>();
                foreach(string user in m_pUsers.Keys){
                    if(m_pUsers[user].CreationTime.AddSeconds(seconds) < DateTime.Now){
                        usersToRemove.Add(user);
                    }
                }
                foreach(string user in usersToRemove){
                    m_pUsers.Remove(user);
                }
            }

            #endregion

            #region method IsEmpty

            /// <summary>
            /// Gets if this entry doesn't contain no user entries.
            /// </summary>
            /// <returns></returns>
            public bool IsEmpty()
            {
                return m_pUsers.Count == 0;
            }

            #endregion


            #region Properties Implementation

            /// <summary>
            /// Gets IP address what is related to this entry.
            /// </summary>
            public string IP
            {
                get{ return m_IP; }
            }

            /// <summary>
            /// Gets time when this entry was created.
            /// </summary>
            public DateTime CreationTime
            {
                get{ return m_CreationTime; }
            }

            #endregion

        }

        #endregion

        private Dictionary<string,BadLoginEntry> m_pEntries     = null;
        private int                              m_MaxBadLogins = 3;
        private Timer                            m_pTimer       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BadLoginManager()
        {
            m_pEntries = new Dictionary<string,BadLoginEntry>();

            m_pTimer = new Timer();
            m_pTimer.Interval = 30000;
            m_pTimer.Elapsed += new ElapsedEventHandler(m_pTimer_Elapsed);
            m_pTimer.Enabled = true;
        }
                
        #region method Dispose

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(m_pTimer != null){
                m_pTimer.Enabled = false;
                m_pTimer = null;
            }
        }

        #endregion


        #region Events Handling

        #region method m_pTimer_Elapsed

        private void m_pTimer_Elapsed(object sender,ElapsedEventArgs e)
        {
            // Remove timedout entries
            try{
                lock(this){
                    List<string> entriesToRemove = new List<string>();
                    foreach(BadLoginEntry entry in m_pEntries.Values){
                        entry.RemoveOlderThan(30);
                        if(entry.IsEmpty()){
                            entriesToRemove.Add(entry.IP);
                        }
                    }
                    foreach(string ip in entriesToRemove){
                        m_pEntries.Remove(ip);
                    }
                }
            }
            catch{
            }
        }

        #endregion

        #endregion


        #region method Put

        /// <summary>
        /// Increases specified IP bad login count for specified user name.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="userName">User name.</param>
        public void Put(string ip,string userName)
        {
            lock(m_pEntries){
                if(!m_pEntries.ContainsKey(ip)){
                    m_pEntries.Add(ip,new BadLoginEntry(ip));                    
                }

                m_pEntries[ip].IncreaseBadLoginCount(userName);
            }
        }

        #endregion

        #region method IsExceeded

        /// <summary>
        /// Gets if maximum allowed bad logins for specified IP and user name has exceeded.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="userName">User name.</param>
        /// <returns></returns>
        public bool IsExceeded(string ip,string userName)
        {
            if(m_pEntries.ContainsKey(ip)){
                return m_pEntries[ip].GetUserBadLoginCount(userName) > m_MaxBadLogins;
            }
            else{
                return false;
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets how many bad logins may be from 1 IP for specified user in specified period.
        /// </summary>
        public int MaximumBadLogins
        {
            get{ return m_MaxBadLogins; }

            set{
                if(m_MaxBadLogins != value){
                    m_MaxBadLogins = value;
                }
            }
        }

        #endregion

    }
}
