using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Specifies match action done when global message rule is matched.
    /// </summary>
    public enum GlobalMessageRuleAction_enum
    {
        /// <summary>
        /// AutoResponse is sent to specified recipient.
        /// </summary>
        AutoResponse = 1,

        /// <summary>
        /// Deletes message.
        /// </summary>
        DeleteMessage = 2,

        /// <summary>
        /// Message is forwarded to specified email.
        /// </summary>
        ForwardToEmail = 3,

        /// <summary>
        /// Message is forwarded to specified host.
        /// </summary>
        ForwardToHost = 4,

        /// <summary>
        /// Message is stored to specified disk folder.
        /// </summary>
        StoreToDiskFolder = 5,

        /// <summary>
        /// Specified program is executed.
        /// </summary>
        ExecuteProgram = 6,

        /// <summary>
        /// Stores message to specified message folder(for example: Junk). This will take effect only for local recipients only !
        /// </summary>
        StoreToIMAPFolder = 7,

        /// <summary>
        /// Appends specified header field to message.
        /// </summary>
        AddHeaderField = 8,

        /// <summary>
        /// Removes specified header field or fields if there are multiple of them.
        /// </summary>
        RemoveHeaderField = 9,

        /// <summary>
        /// Sends error to currently connected client. NOTE: Error text may contain ASCII printable chars only and maximum length is 500.
        /// </summary>
        SendErrorToClient = 10, 

        /// <summary>
        /// Stores message to specified FTP server folder.
        /// </summary>
        StoreToFTPFolder = 11,

        /// <summary>
        /// Posts message to specified NNTP newsgroup.
        /// </summary>
        PostToNNTPNewsGroup = 12,

        /// <summary>
        /// Posts message to specified page via HTTP.
        /// </summary>
        PostToHTTP = 13,
    }
}
