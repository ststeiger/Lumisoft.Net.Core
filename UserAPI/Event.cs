using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Event object represents user event in LumiSoft Mail Server server.
    /// </summary>
    public class Event
    {
        private string         m_ID            = "";        
        private EventType_enum m_EventType     = EventType_enum.Error;
        private string         m_VirtualServer = "";
        private DateTime       m_CreateDate;
        private string         m_Message       = "";
        private string         m_Text          = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">Event ID.</param>
        /// <param name="type">Event type.</param>
        /// <param name="virtualServer">Virtual server name or "" if global error.</param>
        /// <param name="createDate">Event create date.</param>
        /// <param name="message">Event message.</param>
        /// <param name="text">Event extended info.</param>
        internal Event(string id,EventType_enum type,string virtualServer,DateTime createDate,string message,string text)
        {
            m_ID            = id;
            m_EventType     = type;
            m_VirtualServer = virtualServer;
            m_CreateDate    = createDate;
            m_Message       = message;
            m_Text          = text;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets event ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets event type.
        /// </summary>
        public EventType_enum Type
        {
            get{ return m_EventType; }
        }

        /// <summary>
        /// Gets virtual server name or "" if global error.
        /// </summary>
        public string VirtualServer
        {
            get{ return m_VirtualServer; }
        }

        /// <summary>
        /// Gets when events is created.
        /// </summary>
        public DateTime CreateDate
        {
            get{ return m_CreateDate; }
        }
        /*
        /// <summary>
        /// Gets event message.
        /// </summary>
        public string Message
        {
            get{ return m_Message; }
        }*/

        /// <summary>
        /// Gets event extended info.
        /// </summary>
        public string Text
        {
            get{ return m_Text; }
        }

        #endregion

    }
}
