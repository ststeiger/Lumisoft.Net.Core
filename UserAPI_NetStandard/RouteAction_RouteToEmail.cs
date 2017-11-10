using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    ///  The RouteAction_RouteToEmail object represents route "Route To Email" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class RouteAction_RouteToEmail : RouteActionBase
    {
        private string m_Email = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="emailAddress">Wmail address where to route message.</param>
        public RouteAction_RouteToEmail(string emailAddress) : base(RouteAction_enum.RouteToEmail)
        {
            m_Email = emailAddress;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="actionData">Action data.</param>
        internal RouteAction_RouteToEmail(byte[] actionData) : base(RouteAction_enum.RouteToEmail)
        {
            /*  Action data structure:
                    <ActionData>
                        <EmailAddress></EmailAddress>
                    </ActionData>
            */
                     
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Email = table.GetValue("EmailAddress");
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
            table.Add("EmailAddress",m_Email);

            return table.ToByteData();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets email address where to route message.
        /// </summary>
        public string EmailAddress
        {
            get{ return m_Email; }
        }

        #endregion

    }
}
