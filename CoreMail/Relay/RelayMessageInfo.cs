using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.SMTP;

namespace LumiSoft.MailServer.Relay
{
    /// <summary>
    /// This class holds relay message internal info.
    /// </summary>
    public class RelayMessageInfo
    {
        private string          m_EnvelopeID                = null;
        private string          m_Sender                    = "";
        private string          m_Recipient                 = "";
        private string          m_OriginalRecipient         = null;
        private SMTP_DSN_Notify m_DSN_Notify                = SMTP_DSN_Notify.NotSpecified;
        private SMTP_DSN_Ret    m_DSN_Ret                   = SMTP_DSN_Ret.NotSpecified;
        private DateTime        m_Date;
        private bool            m_DelayedDeliveryNotifySent = false;
        private HostEndPoint    m_pHostEndPoint             = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="envelopeID">Envelope ID_(MAIL FROM: ENVID).</param>
        /// <param name="sender">Senders email address.</param>
        /// <param name="recipient">Recipients email address.</param>
        /// <param name="originalRecipient">Original recipient(RCPT TO: ORCPT).</param>
        /// <param name="notify">DSN notify condition.</param>
        /// <param name="ret">Specifies what parts of message are returned in DSN report.</param>
        /// <param name="date">Message date.</param>
        /// <param name="delayedDeliveryNotifySent">Specifies if delayed delivery notify has been sent.</param>
        /// <param name="hostEndPoint">Host end point where message must be sent. Value null means DNS is used to get message target host.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sender</b> or <b>recipient</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public RelayMessageInfo(string envelopeID,string sender,string recipient,string originalRecipient,SMTP_DSN_Notify notify,SMTP_DSN_Ret ret,DateTime date,bool delayedDeliveryNotifySent,HostEndPoint hostEndPoint)
        {
            if(sender == null){
                throw new ArgumentNullException("sender");
            }
            if(recipient == null){
                throw new ArgumentNullException("recipient");
            }
            if(recipient == ""){
                throw new ArgumentException("Argument 'recipient' value must be specified.");
            }

            m_EnvelopeID                = envelopeID;
            m_Sender                    = sender;
            m_Recipient                 = recipient;
            m_OriginalRecipient         = originalRecipient;
            m_DSN_Notify                = notify;
            m_DSN_Ret                   = ret;
            m_Date                      = date;
            m_DelayedDeliveryNotifySent = delayedDeliveryNotifySent;
            m_pHostEndPoint             = hostEndPoint;
        }


        #region static method Parse

        /// <summary>
        /// Parses RelayMessageInfo from byte[] data.
        /// </summary>
        /// <param name="value">RelayMessageInfo data.</param>
        /// <returns>Returns parsed RelayMessageInfo.</returns>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public static RelayMessageInfo Parse(byte[] value)
        {
            try{
                XmlTable messageInfo = new XmlTable("RelayMessageInfo");
                messageInfo.Parse(value);

                return new RelayMessageInfo(
                    string.IsNullOrEmpty(messageInfo.GetValue("EnvelopeID")) ? null : messageInfo.GetValue("EnvelopeID"),
                    messageInfo.GetValue("Sender"), 
                    messageInfo.GetValue("Recipient"), 
                    string.IsNullOrEmpty(messageInfo.GetValue("OriginalRecipient")) ? null : messageInfo.GetValue("OriginalRecipient"),
                    string.IsNullOrEmpty(messageInfo.GetValue("DSN-Notify")) ? SMTP_DSN_Notify.NotSpecified : (SMTP_DSN_Notify)Enum.Parse(typeof(SMTP_DSN_Notify),messageInfo.GetValue("DSN-Notify")),
                    string.IsNullOrEmpty(messageInfo.GetValue("DSN-RET")) ? SMTP_DSN_Ret.NotSpecified : (SMTP_DSN_Ret)Enum.Parse(typeof(SMTP_DSN_Ret),messageInfo.GetValue("DSN-RET")),
                    DateTime.ParseExact(messageInfo.GetValue("Date"),"r",System.Globalization.DateTimeFormatInfo.InvariantInfo), 
                    Convert.ToBoolean(messageInfo.GetValue("DelayedDeliveryNotifySent")), 
                    !string.IsNullOrEmpty(messageInfo.GetValue("HostEndPoint")) ? HostEndPoint.Parse(messageInfo.GetValue("HostEndPoint"),25) :  null
                );
            }
            catch{
                throw new ArgumentException("Argument 'value' has invalid RelayMessageInfo value.");
            }
        }

        #endregion


        #region method ToByte

        /// <summary>
        /// Stores RelayMessageInfo to byte[].
        /// </summary>
        /// <returns>Returns RelayMessageInfo as byte[] data.</returns>
        public byte[] ToByte()
        {
            XmlTable retVal = new XmlTable("RelayMessageInfo");
            if(!string.IsNullOrEmpty(m_EnvelopeID)){
                retVal.Add("EnvelopeID",m_EnvelopeID);
            }
            retVal.Add("Sender",m_Sender);
            retVal.Add("Recipient",m_Recipient);
            if(!string.IsNullOrEmpty(m_OriginalRecipient)){
                retVal.Add("OriginalRecipient",m_OriginalRecipient);
            }
            if(m_DSN_Notify != SMTP_DSN_Notify.NotSpecified){
                retVal.Add("DSN-Notify",m_DSN_Notify.ToString());
            }
            if(m_DSN_Ret != SMTP_DSN_Ret.NotSpecified){
                retVal.Add("DSN-RET",m_DSN_Ret.ToString());
            }
            retVal.Add("Date",m_Date.ToString("r"));
            retVal.Add("DelayedDeliveryNotifySent",m_DelayedDeliveryNotifySent.ToString());
            if(m_pHostEndPoint != null){
                retVal.Add("HostEndPoint",m_pHostEndPoint.ToString());
            }
            else{
                retVal.Add("HostEndPoint","");
            }

            return retVal.ToByteData();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets envelope ID(MAIL FROM: ENVID). Value null means not specified.
        /// </summary>
        public string EnvelopeID
        {
            get{ return m_EnvelopeID; }
        }

        /// <summary>
        /// Gets sender email address.
        /// </summary>
        public string Sender
        {
            get{ return m_Sender; }
        }

        /// <summary>
        /// Gets recipient email address.
        /// </summary>
        public string Recipient
        {
            get{ return m_Recipient; }
        }

        /// <summary>
        /// Gets message original recipient(RCPT TO: ORCPT). Value null means not specified.
        /// </summary>
        public string OriginalRecipient
        {
            get{ return m_OriginalRecipient; }
        }

        /// <summary>
        /// Gets message receive date.
        /// </summary>
        public DateTime Date
        {
            get{ return m_Date; }
        }

        /// <summary>
        /// Gets DSN Notify value.
        /// </summary>
        public SMTP_DSN_Notify DSN_Notify
        {
            get{ return m_DSN_Notify; }
        }

        /// <summary>
        /// Gets DSN RET value.
        /// </summary>
        public SMTP_DSN_Ret DSN_Ret
        {
            get{ return m_DSN_Ret; }
        }

        /// <summary>
        /// Gets or sets if delayed delivery notify sent.
        /// </summary>
        public bool DelayedDeliveryNotifySent
        {
            get{ return m_DelayedDeliveryNotifySent; }

            set{ m_DelayedDeliveryNotifySent = value; }
        }

        /// <summary>
        /// Gets host end point where message must be sent. Value null means DNS is used to get message target host.
        /// </summary>
        public HostEndPoint HostEndPoint
        {
            get{ return m_pHostEndPoint; }
        }

        #endregion

    }
}
