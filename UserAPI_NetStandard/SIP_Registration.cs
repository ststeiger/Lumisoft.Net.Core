using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SipRegistration object represents SIP registration in LumiSoft Mail Server server.
    /// </summary>
    public class SipRegistration
    {
        private SipRegistrationCollection m_pOwner          = null;
        private string                    m_UserName        = "";
        private string                    m_AddressOfRecord = "";
        private SipRegistrationContact[]  m_pContacts       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner SipRegistrationCollection collection that owns this object.</param>
        /// <param name="userName">User name of user which added this registration.</param>
        /// <param name="addressOfRecord">SIP registration address of record.</param>
        /// <param name="contacts">SIP registration contacts.</param>
        internal SipRegistration(SipRegistrationCollection owner,string userName,string addressOfRecord,SipRegistrationContact[] contacts)
        {
            m_pOwner          = owner;
            m_UserName        = userName;
            m_AddressOfRecord = addressOfRecord;
            m_pContacts       = contacts;
        }


        #region method Refresh

        /// <summary>
        /// Refreshes specified registration info.
        /// </summary>
        public void Refresh()
        {
            /* GetSipRegistration "<virtualServerID>" "<addressOfRecord>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pOwner.VirtualServer.Server.LockSynchronizer){
                m_pOwner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetSipRegistration " + 
                    TextUtils.QuoteString(m_pOwner.VirtualServer.VirtualServerID) + " " +
                    TextUtils.QuoteString(m_AddressOfRecord)
                );

                string response = m_pOwner.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pOwner.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
             
                if(ds.Tables.Contains("Contacts")){
                    List<SipRegistrationContact> contacts = new List<SipRegistrationContact>();
                    foreach(DataRow dr in ds.Tables["Contacts"].Rows){
                        SIP_t_ContactParam c = new SIP_t_ContactParam();
                        c.Parse(new LumiSoft.Net.StringReader(dr["Value"].ToString()));
                        contacts.Add(new SipRegistrationContact(c.Address.Uri.Value,c.Expires,c.QValue));
                    }
                    m_pContacts = contacts.ToArray();
                }
                else{
                    m_pContacts = new SipRegistrationContact[0];
                }
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner registration collection.
        /// </summary>
        public SipRegistrationCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets user name of user which added this registration.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }
        }
        
        /// <summary>
        /// Gets SIP registration address of record. This is user ad registration name/id.
        /// </summary>
        public string AddressOfRecord
        {
            get{ return m_AddressOfRecord; }
        }

        /// <summary>
        /// Gets this registration contacts.
        /// </summary>
        public SipRegistrationContact[] Contacts
        {
            get{ return m_pContacts; }
        }

        #endregion

    }
}
