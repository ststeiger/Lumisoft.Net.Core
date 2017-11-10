using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;
using LumiSoft.Net.SMTP;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// This class is used to generate delivery status notifications(DSN). Defined in RFC 3464.
    /// </summary>
    public class DeliveryStatusNotification
    {
        #region static method CreateDsnMessage

        /// <summary>
        /// Creates delivery status notifications(DSN) message. 
        /// </summary>
        /// <param name="to">DSN message To.</param>
        /// <param name="subject">DSN message subject.</param>
        /// <param name="rtfText">DSN message RTF body text.</param>
        /// <param name="envelopeID">Envelope ID(MAIL FROM: ENVID).</param>
        /// <param name="arrivalDate">Message arrival date.</param>
        /// <param name="receivedFromMTA">The remote host EHLo name from where messages was received.</param>
        /// <param name="reportingMTA">Reporting MTA name.</param>
        /// <param name="originalRecipient">Original recipient(RCPT TO: ORCTP).</param>
        /// <param name="finalRecipient">Final recipient.</param>
        /// <param name="action">DSN action.</param>
        /// <param name="statusCode_text">Remote SMTP status code with text.</param>
        /// <param name="remoteMTA">Remote MTA what returned <b>statusCode_text</b>.</param>
        /// <param name="lastAttempt">Last delivery attempt.</param>
        /// <param name="retryUntil">Date time how long server will attempt to deliver message.</param>
        /// <param name="ret">Specifies what original message part are renturned.</param>
        /// <param name="message">Original message.</param>
        /// <returns>Returns created DSN message.</returns>
        public static Mail_Message CreateDsnMessage(string to,string subject,string rtfText,string envelopeID,DateTime arrivalDate,string receivedFromMTA,string reportingMTA,string originalRecipient,string finalRecipient,string action,string statusCode_text,string remoteMTA,DateTime lastAttempt,DateTime retryUntil,SMTP_DSN_Ret ret,Mail_Message message)
        {
            // For more info, see RFC 3464.

            // Ensure that all line-feeds are CRLF.
            rtfText = rtfText.Replace("\r\n","\n").Replace("\n","\r\n");

            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.Date = DateTime.Now;
            msg.From = new Mail_t_MailboxList();
            msg.From.Add(new Mail_t_Mailbox("Mail Delivery Subsystem","postmaster@local"));
            msg.To = new Mail_t_AddressList();
            msg.To.Add(new Mail_t_Mailbox(null,to));
            msg.Subject = subject;

            //--- multipart/report -------------------------------------------------------------------------------------------------
            MIME_h_ContentType contentType_multipartReport = new MIME_h_ContentType(MIME_MediaTypes.Multipart.report);            
            contentType_multipartReport.Parameters["report-type"] = "delivery-status";
            contentType_multipartReport.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
            MIME_b_MultipartReport multipartReport = new MIME_b_MultipartReport(contentType_multipartReport);
            msg.Body = multipartReport;

                //--- multipart/alternative -----------------------------------------------------------------------------------------
                MIME_Entity entity_multipart_alternative = new MIME_Entity();
                MIME_h_ContentType contentType_multipartAlternative = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative);
                contentType_multipartAlternative.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
                MIME_b_MultipartAlternative multipartAlternative = new MIME_b_MultipartAlternative(contentType_multipartAlternative);
                entity_multipart_alternative.Body = multipartAlternative;
                multipartReport.BodyParts.Add(entity_multipart_alternative);

                    //--- text/plain ---------------------------------------------------------------------------------------------------
                    MIME_Entity entity_text_plain = new MIME_Entity();
                    MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                    entity_text_plain.Body = text_plain;
                    text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,SCore.RtfToText(rtfText));
                    multipartAlternative.BodyParts.Add(entity_text_plain);

                    //--- text/html -----------------------------------------------------------------------------------------------------
                    MIME_Entity entity_text_html = new MIME_Entity();
                    MIME_b_Text text_html = new MIME_b_Text(MIME_MediaTypes.Text.html);
                    entity_text_html.Body = text_html;
                    text_html.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,SCore.RtfToHtml(rtfText));
                    multipartAlternative.BodyParts.Add(entity_text_html);

                //--- message/delivery-status
                MIME_Entity entity_message_deliveryStatus = new MIME_Entity();
                MIME_b_Message body_message_deliveryStatus = new MIME_b_Message(MIME_MediaTypes.Message.delivery_status);
                entity_message_deliveryStatus.Body = body_message_deliveryStatus;                
                StringBuilder dsnText = new StringBuilder();
                if(!string.IsNullOrEmpty(envelopeID)){
                    dsnText.Append("Original-Envelope-Id: " + envelopeID + "\r\n");
                }
                dsnText.Append("Arrival-Date: " + MIME_Utils.DateTimeToRfc2822(arrivalDate) + "\r\n");
                if(!string.IsNullOrEmpty(receivedFromMTA)){
                    dsnText.Append("Received-From-MTA: dns; " + receivedFromMTA + "\r\n");
                }
                dsnText.Append("Reporting-MTA: dns; " + reportingMTA + "\r\n");
                dsnText.Append("\r\n");
                if(!string.IsNullOrEmpty(originalRecipient)){
                    dsnText.Append("Original-Recipient: " + originalRecipient + "\r\n");
                }
                dsnText.Append("Final-Recipient: rfc822;" + finalRecipient + "" + "\r\n");
                dsnText.Append("Action: " + action + "\r\n");
                dsnText.Append("Status: " + statusCode_text.Substring(0,1) + ".0.0" + "\r\n");
                if(!string.IsNullOrEmpty(statusCode_text)){
                    dsnText.Append("Diagnostic-Code: smtp; " + statusCode_text + "\r\n");
                }
                if(!string.IsNullOrEmpty(remoteMTA)){
                    dsnText.Append("Remote-MTA: dns; " + remoteMTA + "\r\n");
                }
                if(lastAttempt != DateTime.MinValue){
                    dsnText.Append("Last-Attempt-Date: " + MIME_Utils.DateTimeToRfc2822(lastAttempt) + "\r\n");
                }
                if(retryUntil != DateTime.MinValue){
                    dsnText.Append("Will-Retry-Until: " + MIME_Utils.DateTimeToRfc2822(retryUntil) + "\r\n");
                }
                dsnText.Append("\r\n");
                body_message_deliveryStatus.SetData(new MemoryStream(Encoding.UTF8.GetBytes(dsnText.ToString())),MIME_TransferEncodings.EightBit);
                multipartReport.BodyParts.Add(entity_message_deliveryStatus);

                //--- message/rfc822
                if(message != null){
                    MIME_Entity entity_message_rfc822 = new MIME_Entity();
                    MIME_b_MessageRfc822 body_message_rfc822 = new MIME_b_MessageRfc822();
                    entity_message_rfc822.Body = body_message_rfc822;
                    if(ret == SMTP_DSN_Ret.FullMessage){
                        body_message_rfc822.Message = message;
                    }
                    else{
                        MemoryStream ms = new MemoryStream();
                        message.Header.ToStream(ms,null,null);
                        ms.Position = 0;
                        body_message_rfc822.Message = Mail_Message.ParseFromStream(ms);
                    }                    
                    multipartReport.BodyParts.Add(entity_message_rfc822);
                }

            return msg;
        }

        #endregion
    }
}
