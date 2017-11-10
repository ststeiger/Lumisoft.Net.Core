using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    ///  The RouteAction_RouteToMailbox object represents route "Route To Mailbox" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class RouteAction_RouteToMailbox : RouteActionBase
    {
        private string m_Mailbox = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailbox">Mailvox where to route message.</param>
        public RouteAction_RouteToMailbox(string mailbox) : base(RouteAction_enum.RouteToMailbox)
        {
            m_Mailbox = mailbox;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="actionData">Action data.</param>
        internal RouteAction_RouteToMailbox(byte[] actionData) : base(RouteAction_enum.RouteToMailbox)
        {
            /*  Action data structure:
                    <ActionData>
                        <Mailbox></Mailbox>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Mailbox = table.GetValue("Mailbox");
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
                        <EmailAddress></EmailAddress>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Mailbox",m_Mailbox);

            return table.ToByteData();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets mailbox where to route message.
        /// </summary>
        public string Mailbox
        {
            get{ return m_Mailbox; }
        }

        #endregion

    }
}
