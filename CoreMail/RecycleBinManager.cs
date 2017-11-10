using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Timers;

using LumiSoft;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Recycle bin messages manager.
    /// </summary>
    internal class RecycleBinManager : IDisposable
    {
        private IMailServerApi m_pApi   = null;
        private Timer          m_pTimer = null;
        private DateTime       m_LastCleanTime;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">Virtual server api</param>
        public RecycleBinManager(IMailServerApi api)
        {
            m_pApi          = api;
            
            m_pTimer = new Timer();
            m_pTimer.Interval = 1000 * 60 * 60;
            m_pTimer.Elapsed += new ElapsedEventHandler(m_pTimer_Elapsed);
            m_pTimer.Enabled = true;

            m_LastCleanTime = DateTime.MinValue;
        }

        #region method Dispose

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(m_pTimer != null){
                m_pTimer.Dispose();
                m_pTimer = null;
            }
        }

        #endregion


        #region Events Handling

        #region method m_pTimer_Elapsed

        private void m_pTimer_Elapsed(object sender,ElapsedEventArgs e)
        {
            DoCleanUp();
        }

        #endregion

        #endregion


        #region method DoCleanUp

        /// <summary>
        /// Deletes recycle bin messages what 'delete after days' has exceeded.
        /// </summary>
        public void DoCleanUp()
        {
            try{
                int delAferDays = Convert.ToInt32(m_pApi.GetRecycleBinSettings().Rows[0]["DeleteMessagesAfter"]);

                foreach(DataRowView drV in m_pApi.GetRecycleBinMessagesInfo(null,DateTime.MinValue,DateTime.Today.AddDays(-delAferDays))){
                    if(DateTime.Now > Convert.ToDateTime(drV["DeleteTime"]).AddDays(delAferDays)){
                        try{
                            m_pApi.DeleteRecycleBinMessage(drV["MessageID"].ToString());
                        }
                        catch{
                            // RecycelBin message file is probably locked, so skip it now.
                        }
                    }
                }

                m_LastCleanTime = DateTime.Now;
            }
            catch(Exception x){
                Error.DumpError(x,new System.Diagnostics.StackTrace());
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets when last clean up was done.
        /// </summary>
        public DateTime LastCleanUpTime
        {
            get{ return m_LastCleanTime; }
        }

        #endregion

    }
}
