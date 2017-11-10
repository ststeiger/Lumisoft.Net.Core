using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// This is base class for user mesage rule actions.
    /// </summary>
    public class RouteActionBase
    {
        private RouteAction_enum m_ActionType = RouteAction_enum.RouteToMailbox;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="actionType">Route action type.</param>
        internal RouteActionBase(RouteAction_enum actionType)
        {
            m_ActionType = actionType;
        }


        #region method Serialize

        /// <summary>
        /// Serialices action object.
        /// </summary>
        /// <returns>Returns serialized action data.</returns>
        internal virtual byte[] Serialize()
        {
            return new byte[0];
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets route action type.
        /// </summary>
        public RouteAction_enum ActionType
        {
            get{ return m_ActionType; }
        }

        #endregion

    }
}
