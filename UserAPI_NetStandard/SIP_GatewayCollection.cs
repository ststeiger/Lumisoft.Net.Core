using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SIP_GatewayCollection object represents SIP Gateways in LumiSoft Mail Server virtual server.
    /// </summary>
    public class SIP_GatewayCollection : IEnumerable
    {
        private System_Settings   m_pOwner      = null;
        private List<SIP_Gateway> m_pCollection = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner system settings.</param>
        internal SIP_GatewayCollection(System_Settings owner)
        {
            m_pOwner      = owner;
            m_pCollection = new List<SIP_Gateway>();
        }


        #region method Add

        /// <summary>
        /// Adds new item to the collection.
        /// </summary>
        /// <param name="uriScheme">URI scheme.</param>
        /// <param name="transport">SIP transport.</param>
        /// <param name="host">Gateway host.</param>
        /// <param name="port">Gateway port.</param>
        /// <param name="realm">Gateway realm.</param>
        /// <param name="userName">Gateway user name.</param>
        /// <param name="password">Gateway password.</param>
        /// <returns>Returns new added item.</returns>
        public SIP_Gateway Add(string uriScheme,string transport,string host,int port,string realm,string userName,string password)
        {
            SIP_Gateway gw = AddInternal(uriScheme,transport,host,port,realm,userName,password);            
            m_pOwner.SetValuesChanged();

            return gw;
        }

        /// <summary>
        /// Adds new item to the collection. This method doesn't set HasChanges falg.
        /// </summary>
        /// <param name="uriScheme">URI scheme.</param>
        /// <param name="transport">SIP transport.</param>
        /// <param name="host">Gateway host.</param>
        /// <param name="port">Gateway port.</param>
        /// <param name="realm">Gateway realm.</param>
        /// <param name="userName">Gateway user name.</param>
        /// <param name="password">Gateway password.</param>
        /// <returns>Returns new added item.</returns>
        internal SIP_Gateway AddInternal(string uriScheme,string transport,string host,int port,string realm,string userName,string password)
        {
            SIP_Gateway gw = new SIP_Gateway(this,uriScheme,transport,host,port,realm,userName,password);
            m_pCollection.Add(gw);

            return gw;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="value">Item to remove.</param>
        public void Remove(SIP_Gateway value)
        {
            m_pCollection.Remove(value);
            m_pOwner.SetValuesChanged();
        }

        #endregion

        #region method Clear

        /// <summary>
        /// Removes all tiems from the collection.
        /// </summary>
        public void Clear()
        {
            m_pCollection.Clear();
            m_pOwner.SetValuesChanged();
        }

        #endregion


        #region interface IEnumerator

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pCollection.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner system settings.
        /// </summary>
        public System_Settings Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get{ return m_pCollection.Count; }
        }

        #endregion

    }
}
