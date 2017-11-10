using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SipRegistrationCollection object represents SIP registrations in LumiSoft Mail Server server.
    /// </summary>
    public class SipRegistrationCollection
    {
        private VirtualServer         m_pOwner         = null;
        private List<SipRegistration> m_pRegistrations = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner VirtualServer object that owns this collection.</param>
        internal SipRegistrationCollection(VirtualServer owner)
        {
            m_pOwner = owner;
            m_pRegistrations = new List<SipRegistration>();

            Bind();
        }


        #region method Refresh

        /// <summary>
        /// Refreshes sessions.
        /// </summary>
        public void Refresh()
        {
            lock(m_pOwner.Server.LockSynchronizer){
                m_pRegistrations.Clear();
                Bind();
            }
        }

        #endregion

        #region method Set

        /// <summary>
        /// Adds or updates specified SIP registration info.
        /// </summary>
        /// <param name="addressOfRecord">Registration address of record.</param>
        /// <param name="contacts">Contacts to add to the specified address of record.</param>
        public void Set(string addressOfRecord,string[] contacts)
        {
            /* SetSipRegistration "<virtualServerID>" "<addressOfRecord>" "<contacts>"
                  Responses:
                    +OK                    
                    -ERR <errorText>
            */

            lock(m_pOwner.Server.LockSynchronizer){
                // Build TAB delimited contacts.
                string tabContacts = "";
                for(int i=0;i<contacts.Length;i++){
                    // Don't add TAB to last item.
                    if(i == (contacts.Length - 1)){
                        tabContacts += contacts[i];
                    }
                    else{
                        tabContacts += contacts[i] + "\t";
                    }
                }
            
                m_pOwner.Server.TcpClient.TcpStream.WriteLine("SetSipRegistration " +
                    TextUtils.QuoteString(m_pOwner.VirtualServerID) + " " +
                    TextUtils.QuoteString(addressOfRecord) + " " + 
                    TextUtils.QuoteString(tabContacts)
                );

                string response = m_pOwner.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                SipRegistration registration = new SipRegistration(this,"administrator",addressOfRecord,new SipRegistrationContact[0]);
                m_pRegistrations.Add(registration);
                // Force to registration to get new registration info from server.
                registration.Refresh();
            }
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified SIP registration from server,
        /// </summary>
        /// <param name="registration">Registration to remove.</param>
        public void Remove(SipRegistration registration)
        {
            /* DeleteSipRegistration "<virtualServerID>" "<addressOfRecord>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            lock(m_pOwner.Server.LockSynchronizer){
                string id = Guid.NewGuid().ToString();

                // Call TCP DeleteSipRegistration
                m_pOwner.Server.TcpClient.TcpStream.WriteLine("DeleteSipRegistration " + 
                    TextUtils.QuoteString(m_pOwner.VirtualServerID) + " " +
                    TextUtils.QuoteString(registration.AddressOfRecord)
                );
                        
                string response = m_pOwner.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                m_pRegistrations.Remove(registration);
            }
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server events and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetSipRegistrations "<virtualServerID>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            lock(m_pOwner.Server.LockSynchronizer){
                m_pOwner.Server.TcpClient.TcpStream.WriteLine("GetSipRegistrations " + 
                    TextUtils.QuoteString(m_pOwner.VirtualServerID)
                );

                string response = m_pOwner.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pOwner.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("SipRegistrations")){
                    foreach(DataRow dr in ds.Tables["SipRegistrations"].Rows){
                        //--- Parse contact -------------------------------------------------------------//
                        List<SipRegistrationContact> contacts = new List<SipRegistrationContact>();
                        foreach(string contact in dr["Contacts"].ToString().Split('\t')){
                            if(!string.IsNullOrEmpty(contact)){
                                SIP_t_ContactParam c = new SIP_t_ContactParam();
                                c.Parse(new LumiSoft.Net.StringReader(contact));
                                contacts.Add(new SipRegistrationContact(c.Address.Uri.Value,c.Expires,c.QValue));
                            }
                        }
                        //--------------------------------------------------------------------------------//

                        m_pRegistrations.Add(new SipRegistration(
                            this,
                            dr["UserName"].ToString(),
                            dr["AddressOfRecord"].ToString(),
                            contacts.ToArray()
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
			return m_pRegistrations.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
        
        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pOwner; }
        }
      
        /// <summary>
        /// Gets number of SIP registrations in server.
        /// </summary>
        public int Count
        {
            get{ return m_pRegistrations.Count; }
        }

        /// <summary>
        /// Gets a SipRegistration object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the SipRegistration object in the SipRegistrationCollection collection.</param>
        /// <returns>A SipRegistration object value that represents the SIP registration in server.</returns>
        public SipRegistration this[int index]
        {
            get{ return m_pRegistrations[index]; }
        }

        /// <summary>
        /// Gets a SipRegistration object in the collection by SIP registration name.
        /// </summary>
        /// <param name="addressOfRecord">A String value that specifies the SIP registration 'address of record' of the SipRegistration object in the SipRegistrationCollection collection.</param>
        /// <returns>A SipRegistration object value that represents the SIP registration in server.</returns>
        public SipRegistration this[string addressOfRecord]
        {
            get{ 
                foreach(SipRegistration registration in m_pRegistrations){
                    if(registration.AddressOfRecord.ToLower() == addressOfRecord.ToLower()){
                        return registration;
                    }
                }

                throw new Exception("SipRegistration with registration '" + addressOfRecord + "' doesn't exist !"); 
            }
        }


        /// <summary>
        /// Gets direct access to sessions collection.
        /// </summary>
        internal List<SipRegistration> List
        {
            get{ return m_pRegistrations; }
        }

        #endregion

    }
}
