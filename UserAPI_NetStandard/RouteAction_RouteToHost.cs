using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    ///  The RouteAction_RouteToHost object represents route "Route To Host" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class RouteAction_RouteToHost : RouteActionBase
    {
        private string m_Host = "";
        private int    m_Port = 25;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="host">Host name or IP where to route message.</param>
        /// <param name="port">Host port.</param>
        public RouteAction_RouteToHost(string host,int port) : base(RouteAction_enum.RouteToHost)
        {
            m_Host = host;
            m_Port = port;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="actionData">Action data.</param>
        internal RouteAction_RouteToHost(byte[] actionData) : base(RouteAction_enum.RouteToHost)
        {
            /*  Action data structure:
                    <ActionData>
                        <Host></Host>
                        <Port></Port>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Host = table.GetValue("Host");
            m_Port = Convert.ToInt32(table.GetValue("Port"));
        }


        #region method Serialize

        /// <summary>
        /// Serialices action object.
        /// </summary>
        /// <returns>Returns serialized action data.</returns>
        internal override byte[] Serialize()
        {
            /*  Action data structure:
                    <ActionData>
                        <Host></Host>
                        <Port></Port>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Host",m_Host);
            table.Add("Port",m_Port.ToString());

            return table.ToByteData();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets host name or IP where to route message.
        /// </summary>
        public string Host
        {
            get{ return m_Host; }
        }

        /// <summary>
        /// Gets or sets host port.
        /// </summary>
        public int Port
        {
            get{ return m_Port; }
        }

        #endregion

    }
}
