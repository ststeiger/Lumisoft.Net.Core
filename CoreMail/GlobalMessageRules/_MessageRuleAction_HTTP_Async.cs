using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// HTTP asynchronous message poster. This class is used by internally by 'Store To HTTP' message rule action.
    /// </summary>
    internal class _MessageRuleAction_HTTP_Async
    {
        private string m_Url      = "";
        private Stream m_pMessage = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="url">Url where to post message. Exmaple: http://domain.com/upload.aspx .</param>
        /// <param name="message">Message to post. Stream position must be where message begins.</param>
        public _MessageRuleAction_HTTP_Async(string url,Stream message)
        {
            m_Url      = url;
            m_pMessage = message;

            Thread tr = new Thread(new ThreadStart(this.Post));
            tr.Start();
        }


        #region method Post

        /// <summary>
        /// Posts message to web page via HTTP.
        /// </summary>
        private void Post()
        {
            try{
                WebClient http = new WebClient();                
                http.Headers.Add("Content-Type","multipart/form-data; boundary=---------------------8c808e3aebd9294");

                string header  = "-----------------------8c808e3aebd9294\r\n";
                       header += "Content-Disposition: form-data; name=\"file\"; filename=\"mail.eml\"\r\n";
                       header += "Content-Type: application/octet-stream\r\n";
                       header += "\r\n";
                       
                MemoryStream ms = new MemoryStream();
                byte[] buffer = System.Text.Encoding.Default.GetBytes(header);
                ms.Write(buffer,0,buffer.Length);
                SCore.StreamCopy(m_pMessage,ms);
                buffer = System.Text.Encoding.Default.GetBytes("\r\n-----------------------8c808e3aebd9294--\r\n");
                ms.Write(buffer,0,buffer.Length);

                byte[] response = http.UploadData(m_Url,ms.ToArray());
            }
            catch(Exception x){
                Error.DumpError(x,new System.Diagnostics.StackTrace());
            }
        }

        #endregion

    }
}
