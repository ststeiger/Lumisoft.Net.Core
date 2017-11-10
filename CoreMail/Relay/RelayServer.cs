using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using LumiSoft.Net;
using LumiSoft.Net.SMTP;
using LumiSoft.Net.SMTP.Client;
using LumiSoft.Net.SMTP.Relay;
using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;
using LumiSoft.MailServer;

namespace LumiSoft.MailServer.Relay
{
    /// <summary>
    /// This class implements relay server in LumiSoft Mail Server.
    /// </summary>
    public class RelayServer : Relay_Server
    {
        private VirtualServer       m_pVirtualServer             = null;
        private int                 m_RelayInterval              = 30;
        private int                 m_RelayRetryInterval         = 180;
        private int                 m_DelayedDeliveryNotifyAfter = 5;
        private int                 m_UndeliveredAfter           = 60;
        private ServerReturnMessage m_DelayedDeliveryMessage     = null;
        private ServerReturnMessage m_UndeliveredMessage         = null;
        private bool                m_StoreUndelivered           = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>virtualServer</b> is null.</exception>
        public RelayServer(VirtualServer virtualServer)
        {
            if(virtualServer == null){
                throw new ArgumentNullException("virtualServer");
            }

            m_pVirtualServer = virtualServer;

            this.Queues.Add(new Relay_Queue("Relay"));
            this.Queues.Add(new Relay_Queue("Retry"));
        }


        #region method Start

        /// <summary>
        /// Starts relay server.
        /// </summary>
        public override void Start()
        {
            base.Start();

            Thread tr1 = new Thread(new ThreadStart(this.ProcessRelay));
            tr1.Start();

            Thread tr2 = new Thread(new ThreadStart(this.ProcessRelayRetry));
            tr2.Start();
        }

        #endregion

        #region method StoreRelayMessage

        /// <summary>
        /// Stores message for relay.
        /// </summary>
        /// <param name="id">Message ID. Guid value is suggested.</param>
        /// <param name="envelopeID">Envelope ID_(MAIL FROM: ENVID).</param>
        /// <param name="message">Message to store. Message will be readed from current position of stream.</param>
        /// <param name="targetHost">Target host or IP where to send message. This value can be null, then DNS MX o A record is used to deliver message.</param>
        /// <param name="sender">Sender address to report to target server.</param>
        /// <param name="to">Message recipient address.</param>
        /// <param name="originalRecipient">Original recipient(RCPT TO: ORCPT).</param>
        /// <param name="notify">DSN notify condition.</param>
        /// <param name="ret">Specifies what parts of message are returned in DSN report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>id</b>,<b>message</b> or <b>to</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the argumnets has invalid value.</exception>
        public void StoreRelayMessage(string id,String envelopeID,Stream message,HostEndPoint targetHost,string sender,string to,string originalRecipient,SMTP_DSN_Notify notify,SMTP_DSN_Ret ret)
        {
            StoreRelayMessage("Relay",id,envelopeID,DateTime.Now,message,targetHost,sender,to,originalRecipient,notify,ret);
        }

        /// <summary>
        /// Stores message for relay.
        /// </summary>
        /// <param name="queueName">Queue name where to store message.</param>
        /// <param name="id">Message ID. Guid value is suggested.</param>
        /// <param name="envelopeID">Envelope ID_(MAIL FROM: ENVID).</param>
        /// <param name="date">Message date.</param>
        /// <param name="message">Message to store. Message will be readed from current position of stream.</param>
        /// <param name="targetHost">Target host or IP where to send message. This value can be null, then DNS MX o A record is used to deliver message.</param>
        /// <param name="sender">Sender address to report to target server.</param>
        /// <param name="to">Message recipient address.</param>
        /// <param name="originalRecipient">Original recipient(RCPT TO: ORCPT).</param>
        /// <param name="notify">DSN notify condition.</param>
        /// <param name="ret">Specifies what parts of message are returned in DSN report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>queueName</b>,<b>id</b>,<b>message</b> or <b>to</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the argumnets has invalid value.</exception>
        private void StoreRelayMessage(string queueName,string id,string envelopeID,DateTime date,Stream message,HostEndPoint targetHost,string sender,string to,string originalRecipient,SMTP_DSN_Notify notify,SMTP_DSN_Ret ret)
        {
            if(queueName == null){
                throw new ArgumentNullException("queueName");
            }
            if(queueName == ""){
                throw new ArgumentException("Argumnet 'queueName' value must be specified.");
            }
            if(id == null){
                throw new ArgumentNullException("id");
            }
            if(id == ""){
                throw new ArgumentException("Argument 'id' value must be specified.");
            }
            if(message == null){
                throw new ArgumentNullException("message");
            }
            if(to == null){
                throw new ArgumentNullException("to");
            }
            if(to == ""){
                throw new ArgumentException("Argument 'to' value must be specified.");
            }
					
			string path = m_pVirtualServer.MailStorePath + queueName;

			// Check if Directory exists, if not Create
			if(!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}

			// Create relay message.
			using(FileStream fs = File.Create(API_Utlis.PathFix(path + "\\" + id + ".eml"))){
                SCore.StreamCopy(message,fs);
                               
                // Create message info file for the specified relay message.
                RelayMessageInfo messageInfo = new RelayMessageInfo(
                    envelopeID,
                    sender,
                    to,
                    originalRecipient,
                    notify,
                    ret,
                    date,
                    false,
                    targetHost
                );
                File.WriteAllBytes(API_Utlis.PathFix(path + "\\" + id + ".info"),messageInfo.ToByte());
			}
        }

        #endregion


        #region method ProcessRelay

        /// <summary>
        /// This loop method processes "Relay" messages while server is running.
        /// </summary>
        private void ProcessRelay()
        {
            DateTime lastRelayTime = DateTime.MinValue;
            while(this.IsRunning){
                try{
                    // Relay interval reached, relay available messages.
                    if(lastRelayTime.AddSeconds(m_RelayInterval) < DateTime.Now){
                        string path = m_pVirtualServer.MailStorePath + "Relay";
                        if(Directory.Exists(path)){
                            string[] messages = Directory.GetFiles(path,"*.eml");                                
                            foreach(string message in messages){
                                // Relay server maximum connections exceeded, we have maxConnections + 25 messages.
                                // Just wait when some messge completes processing.
                                while(this.Queues[0].Count > 25){
                                    if(!this.IsRunning){
                                        return;
                                    }
                                    Thread.Sleep(100);
                                }

                                try{
                                    string     messageID     = Path.GetFileNameWithoutExtension(message);
                                    FileStream messageStream = File.Open(message,FileMode.Open,FileAccess.ReadWrite,FileShare.Read | FileShare.Delete);
                                    if(File.Exists(API_Utlis.PathFix(path + "\\" + messageID + ".info"))){
                                        RelayMessageInfo messageInfo = RelayMessageInfo.Parse(File.ReadAllBytes(API_Utlis.PathFix(path + "\\" + messageID + ".info")));
                                        
                                        // Queue message for relay.
                                        this.Queues[0].QueueMessage(
                                            messageInfo.HostEndPoint == null ? null : new Relay_SmartHost(messageInfo.HostEndPoint.Host,messageInfo.HostEndPoint.Port),
                                            messageInfo.Sender,
                                            messageInfo.EnvelopeID,
                                            messageInfo.DSN_Ret,
                                            messageInfo.Recipient,
                                            messageInfo.OriginalRecipient,
                                            messageInfo.DSN_Notify,
                                            messageID,
                                            messageStream,
                                            messageInfo
                                        );
                                    }
                                    // Relay message info file missing.
                                    else{
                                        LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,new Exception("Relay message '" + message + "' .info file is missing, deleting message."));
                                        File.Delete(message);
                                    }                                    
                                }
                                catch(IOException x){
                                    // Message file is in use, probably is relayed now or being stored for relay, skip it.
                                    string dummy = x.Message;
                                }
                            }
                        }
                        lastRelayTime = DateTime.Now;

                        // We need to sleep, because if relay interval = 0, loop will consume all CPU.
                        Thread.Sleep(1000);
                    }
                    // Not a relay interval yet, sleep.
                    else{
                        Thread.Sleep(1000);
                    }
                }
                catch(Exception x){
                    LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
                }                
            }
        }

        #endregion

        #region method ProcessRelayRetry
        
        /// <summary>
        /// This loop method processes "Relay Retry" messages while server is running.
        /// </summary>
        private void ProcessRelayRetry()
        {
            DateTime lastRelayTime = DateTime.MinValue;
            while(this.IsRunning){
                try{
                    // Relay interval reached, relay available messages.
                    if(lastRelayTime.AddSeconds(m_RelayRetryInterval) < DateTime.Now){
                        string path = m_pVirtualServer.MailStorePath + "Retry";
                        if(Directory.Exists(path)){
                            string[] messages = Directory.GetFiles(path,"*.eml");                                
                            foreach(string message in messages){
                                // Relay server maximum connections exceeded, we have maxConnections + 25 messages.
                                // Just wait when some messge completes processing.
                                while(this.Queues[1].Count > 25){
                                    if(!this.IsRunning){
                                        return;
                                    }
                                    Thread.Sleep(100);
                                }

                                try{
                                    string     messageID     = Path.GetFileNameWithoutExtension(message);
                                    FileStream messageStream = File.Open(message,FileMode.Open,FileAccess.ReadWrite,FileShare.Read | FileShare.Delete);
                                    if(File.Exists(API_Utlis.PathFix(path + "\\" + messageID + ".info"))){
                                        RelayMessageInfo messageInfo = RelayMessageInfo.Parse(File.ReadAllBytes(API_Utlis.PathFix(path + "\\" + messageID + ".info")));

                                        // Queue message for relay.
                                        this.Queues[1].QueueMessage(
                                            messageInfo.Sender,
                                            messageInfo.Recipient,
                                            messageID,
                                            messageStream,
                                            messageInfo
                                        );
                                    }
                                    // Relay message info file missing.
                                    else{
                                        LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,new Exception("Relay message '" + message + "' .info file is missing, deleting message."));
                                        File.Delete(message);
                                    }                                    
                                }
                                catch(IOException x){
                                    // Message file is in use, probably is relayed now or being stored for relay, skip it.
                                    string dummy = x.Message;
                                }
                            }
                        }
                        lastRelayTime = DateTime.Now;

                        // We need to sleep, because if relay interval = 0, loop will consume all CPU.
                        Thread.Sleep(1000);
                    }
                    // Not a relay interval yet, sleep.
                    else{
                        Thread.Sleep(1000);
                    }
                }
                catch(Exception x){
                    LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
                }                
            }
        }

        #endregion

        #region override method OnSessionCompleted

        /// <summary>
        /// Raises <b>SessionCompleted</b> event.
        /// </summary>
        /// <param name="session">Session what completed processing.</param>
        /// <param name="exception">Exception happened or null if relay completed successfully.</param>
        protected override void OnSessionCompleted(Relay_Session session,Exception exception)
        {
            base.OnSessionCompleted(session,exception);

            try{
                FileStream       messageStream = (FileStream)session.MessageStream;
                RelayMessageInfo messageInfo   = (RelayMessageInfo)session.QueueTag;

                bool deleteMessage = false;
                // Message relayed sucessfully, delete message.
                if(exception == null){
                    deleteMessage = true;

                    Send_DSN_Relayed(session);
                }
                // Message relay failed.
                else{
                    bool permanentError = false;
                    if(exception is SMTP_ClientException){
                        permanentError = ((SMTP_ClientException)exception).IsPermanentError;
                    }

                    // If permanent error or undelivered time reached, send undelivered message to sender and delete message.
                    if(permanentError || messageInfo.Date.AddMinutes(m_UndeliveredAfter) < DateTime.Now){
                        // Send undelivered notify message. 
                        Send_DSN_Failed(session,exception.Message);
                       
                        deleteMessage = true;
                    }
                    else{
                        // Move message to "Retry" queue.
                        if(session.Queue.Name.ToLower() == "relay"){
                            session.MessageStream.Position = 0;
                            StoreRelayMessage(
                                "Retry",
                                Path.GetFileNameWithoutExtension(messageStream.Name),
                                messageInfo.EnvelopeID,
                                messageInfo.Date,
                                session.MessageStream,
                                messageInfo.HostEndPoint,
                                messageInfo.Sender,
                                messageInfo.Recipient,
                                messageInfo.OriginalRecipient,
                                messageInfo.DSN_Notify,
                                messageInfo.DSN_Ret
                            );

                            deleteMessage = true;
                        }
                        else{
                            // See delayed delivery warning must be sent.
                            if(!messageInfo.DelayedDeliveryNotifySent && DateTime.Now > messageInfo.Date.AddMinutes(this.DelayedDeliveryNotifyAfter)){
                                Send_DSN_Delayed(session,exception.Message);
                                
                                // Update relay mesage info UndeliveredWarning flag.
                                messageInfo.DelayedDeliveryNotifySent = true;
                                File.WriteAllBytes(messageStream.Name.Replace(".eml",".info"),messageInfo.ToByte());
                            }
                        }
                    }                    
                }

                if(deleteMessage){
                    File.Delete(messageStream.Name);
                    File.Delete(messageStream.Name.Replace(".eml",".info"));
                }
                messageStream.Dispose();
            }
            catch(Exception x){
                LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
            }
        }

        #endregion

        #region method override OnError

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Exception happned.</param>
        protected override void OnError(Exception x)
        {
            base.OnError(x);

            LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
        }

        #endregion


        #region method Send_DSN_Failed

        /// <summary>
        /// Creates and sends "Message delivery failed" to message sender.
        /// </summary>
        /// <param name="session">Relay_Session.</param>
        /// <param name="error">Permanent error happened.</param>
        private void Send_DSN_Failed(Relay_Session session,string error)
        {            
            try{
                // No sender specified, can't send notify, just skip it.
                if(string.IsNullOrEmpty(session.From)){
                    return;
                }

                RelayMessageInfo relayInfo = (RelayMessageInfo)session.QueueTag;

                // Send DSN only if user has not specified at all or has specified "failure".
                if(relayInfo.DSN_Notify != SMTP_DSN_Notify.NotSpecified && (relayInfo.DSN_Notify & SMTP_DSN_Notify.Failure) == 0){
                    return;
                }

                session.MessageStream.Position = 0;
                Mail_Message relayMessage = Mail_Message.ParseFromStream(session.MessageStream);
                RelayVariablesManager variablesMgr = new RelayVariablesManager(this,session,error,relayMessage);

                ServerReturnMessage messageTemplate = this.UndeliveredMessage;
                if(messageTemplate == null){
                    string bodyRtf = "" +
                    "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n" +
                    "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
                    "\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n" +
                    "\\par\r\n" +
                    "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
                    "\\par\r\n" +
                    "\\par\r\n" +
                    "\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n" +
                    "\\par\r\n" +
                    "--------\\par\r\n" +
                    "\\par\r\n" +
                    "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
                    "\\par\r\n" +
                    "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
                    "\\lang1061\\f5\\par\r\n" +
                    "\\lang1033\\f2\\par\r\n" +
                    "}\r\n";

                    messageTemplate = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>",bodyRtf);
                }

                string rtf = variablesMgr.Process(messageTemplate.BodyTextRtf);

                Mail_Message dsnMessage = DeliveryStatusNotification.CreateDsnMessage(
                    session.From,
                    variablesMgr.Process(messageTemplate.Subject),
                    rtf,
                    relayInfo.EnvelopeID,
                    relayInfo.Date,
                    null, 
                    (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName,
                    relayInfo.OriginalRecipient,
                    session.To,
                    "failed",
                    error,
                    session.RemoteHostName,
                    DateTime.Now,
                    DateTime.MinValue,
                    (relayInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.FullMessage : relayInfo.DSN_Ret,
                    relayMessage
                );

				using(MemoryStream strm = new MemoryStream()){
					dsnMessage.ToStream(strm,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8);
					m_pVirtualServer.ProcessAndStoreMessage("",new string[]{session.From},strm,null);
				}

                relayMessage.Dispose();
                dsnMessage.Dispose();
            }
            catch(Exception x){
                LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
            }
        }

        #endregion

        #region method Send_DSN_Delayed

        /// <summary>
        /// Creates and sends "Delayed delivery notify" to message sender.
        /// </summary>
        /// <param name="session">Relay_Session.</param>
        /// <param name="error">Error happened.</param>
        private void Send_DSN_Delayed(Relay_Session session,string error)
        {
            try{
                // No sender specified, can't send notify, just skip it.
                if(string.IsNullOrEmpty(session.From)){
                    return;
                }

                RelayMessageInfo relayInfo = (RelayMessageInfo)session.QueueTag;

                // Send DSN only if user has not specified at all or has specified "delay".
                if(relayInfo.DSN_Notify != SMTP_DSN_Notify.NotSpecified && (relayInfo.DSN_Notify & SMTP_DSN_Notify.Delay) == 0){
                    return;
                }

                session.MessageStream.Position = 0;
                Mail_Message relayMessage = Mail_Message.ParseFromStream(session.MessageStream);
                RelayVariablesManager variablesMgr = new RelayVariablesManager(this,session,error,relayMessage);

                ServerReturnMessage messageTemplate = this.DelayedDeliveryMessage;
                if(messageTemplate == null){
                    string bodyRtf = "" +
                    "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n" +
                    "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
                    "\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n" +
                    "\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n" +
                    "\\par\r\n" +
                    "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
                    "\\par\r\n" +
                    "\\par\r\n" +
                    "Please note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n" +
                    "\\par\r\n" +
                    "--------\\par\r\n" +
                    "\\par\r\n" +
                    "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
                    "\\par\r\n" +
                    "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
                    "\\lang1061\\f2\\par\r\n" +
                    "\\pard\\lang1033\\f3\\fs17\\par\r\n" +
                    "}\r\n";

                    messageTemplate = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>",bodyRtf);
                }

                string rtf = variablesMgr.Process(messageTemplate.BodyTextRtf);

                Mail_Message dsnMessage = DeliveryStatusNotification.CreateDsnMessage(
                    session.From,
                    variablesMgr.Process(messageTemplate.Subject),
                    rtf,
                    relayInfo.EnvelopeID,
                    relayInfo.Date,
                    null, 
                    (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName,
                    relayInfo.OriginalRecipient,
                    session.To,
                    "delayed",
                    error,
                    session.RemoteHostName,
                    DateTime.Now,
                    relayInfo.Date.AddMinutes(this.UndeliveredAfter),
                    (relayInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.FullMessage : relayInfo.DSN_Ret,
                    relayMessage
                );

				using(MemoryStream strm = new MemoryStream()){
					dsnMessage.ToStream(strm,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8);
					m_pVirtualServer.ProcessAndStoreMessage("",new string[]{session.From},strm,null);
				}

                relayMessage.Dispose();
                dsnMessage.Dispose();
            }            
            catch(Exception x){
                LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
            }
        }

        #endregion

        #region method Send_DSN_Relayed

        /// <summary>
        /// Sends deliver-status-notification 'success' message.
        /// </summary>
        /// <param name="session">Relay session.</param>
        private void Send_DSN_Relayed(Relay_Session session)
        {
            if(session == null){
                return;
            }

            try{
                // No sender specified, can't send notify, just skip it.
                if(string.IsNullOrEmpty(session.From)){
                    return;
                }

                RelayMessageInfo relayInfo = (RelayMessageInfo)session.QueueTag;

                // Send DSN only if user has specified "success".
                if((relayInfo.DSN_Notify & SMTP_DSN_Notify.Success) == 0){
                    return;
                }

                session.MessageStream.Position = 0;
                Mail_Message relayMessage = Mail_Message.ParseFromStream(session.MessageStream);
                RelayVariablesManager variablesMgr = new RelayVariablesManager(this,session,"",relayMessage);

                ServerReturnMessage messageTemplate = null;
                if(messageTemplate == null){
                    string bodyRtf = "" +
                    "{\\rtf1\\ansi\\ansicpg1257\\deff0\\deflang1061{\\fonttbl{\\f0\\froman\\fcharset0 Times New Roman;}{\\f1\froman\\fcharset186{\\*\\fname Times New Roman;}Times New Roman Baltic;}{\\f2\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n" +
                    "{\\colortbl ;\\red0\\green128\\blue0;\\red128\\green128\\blue128;}\r\n" +
                    "{\\*\\generator Msftedit 5.41.21.2508;}\\viewkind4\\uc1\\pard\\sb100\\sa100\\lang1033\\f0\\fs24\\par\r\n" +
                    "Your message WAS SUCCESSFULLY RELAYED to:\\line\\lang1061\\f1\\tab\\cf1\\lang1033\\b\\f0 <" + session.To + ">\\line\\cf0\\b0 and you explicitly requested a delivery status notification on success.\\par\\par\r\n" +
                    "\\cf2 Your original message\\lang1061\\f1 /header\\lang1033\\f0  is attached to this e-mail\\lang1061\\f1 .\\lang1033\\f0\\par\\r\\n" +
                    "\\cf0\\line\\par\r\n" +
                    "\\pard\\lang1061\\f2\\fs20\\par\r\n" +
                    "}\r\n";

                    messageTemplate = new ServerReturnMessage("DSN SUCCESSFULLY RELAYED: <#message.header[\"Subject:\"]>",bodyRtf);
                }

                string rtf = variablesMgr.Process(messageTemplate.BodyTextRtf);

                Mail_Message dsnMessage = DeliveryStatusNotification.CreateDsnMessage(
                    session.From,
                    variablesMgr.Process(messageTemplate.Subject),
                    rtf,
                    relayInfo.EnvelopeID,
                    relayInfo.Date,
                    null, 
                    (session.IsConnected && string.IsNullOrEmpty(session.LocalHostName)) ? session.LocalEndPoint.Address.ToString() : session.LocalHostName,
                    relayInfo.OriginalRecipient,
                    session.To,
                    "relayed",
                    "200 OK",
                    session.RemoteHostName,
                    DateTime.MinValue,
                    DateTime.MinValue,
                    (relayInfo.DSN_Ret == SMTP_DSN_Ret.NotSpecified) ? SMTP_DSN_Ret.Headers : relayInfo.DSN_Ret,
                    relayMessage
                );

				using(MemoryStream strm = new MemoryStream()){
					dsnMessage.ToStream(strm,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8);
					m_pVirtualServer.ProcessAndStoreMessage("",new string[]{session.From},strm,null);
				}

                relayMessage.Dispose();
                dsnMessage.Dispose();
            }
            catch(Exception x){
                LumiSoft.MailServer.Error.DumpError(m_pVirtualServer.Name,x);
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets relay interval seconds.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int RelayInterval
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_RelayInterval; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_RelayInterval != value){
                    m_RelayInterval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets relay retry interval seconds.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int RelayRetryInterval
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_RelayRetryInterval; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_RelayRetryInterval != value){
                    m_RelayRetryInterval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets after how many minutes delayed delivery notify is sent.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int DelayedDeliveryNotifyAfter
        {
            get{ 
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_DelayedDeliveryNotifyAfter; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_DelayedDeliveryNotifyAfter != value){
                    m_DelayedDeliveryNotifyAfter = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets after how many minutes message is considered undelivered. 
        /// Undelivered notification is sent to sender.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int UndeliveredAfter
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_UndeliveredAfter; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_UndeliveredAfter != value){
                    m_UndeliveredAfter = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets message template what is sent when message delayed delivery, immediate delivery failed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public ServerReturnMessage DelayedDeliveryMessage
        {
            get{ 
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_DelayedDeliveryMessage; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_DelayedDeliveryMessage != value){
                    m_DelayedDeliveryMessage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets message template what is sent when message delivery has failed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public ServerReturnMessage UndeliveredMessage
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_UndeliveredMessage; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_UndeliveredMessage != value){
                    m_UndeliveredMessage = value;
                }
            }
        }        

        /// <summary>
        /// Gets or sets if undelivered messages are stored.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool StoreUndeliveredMessages
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_StoreUndelivered; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                if(m_StoreUndelivered != value){
                    m_StoreUndelivered = value;
                }
            }
        }

        #endregion

    }
}
