using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_StoreToFtp object represents global message rule "Store To FTP Folder" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_StoreToFtp : GlobalMessageRuleActionBase
    {
        private string m_Server   = "";
        private int    m_Port     = 21;
        private string m_UserName = "";
        private string m_Password = "";
        private string m_Folder   = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal GlobalMessageRuleAction_StoreToFtp(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <Server></Server>
                        <Port></Server>
                        <User></User>
                        <Password></Password>
                        <Folder></Folder>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Server   = table.GetValue("Server");
            m_Port     = Convert.ToInt32(table.GetValue("Port"));
            m_UserName = table.GetValue("User");
            m_Password = table.GetValue("Password");
            m_Folder   = table.GetValue("Folder");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="host">FTP server where to store message.</param>
        /// <param name="port">FTP server port.</param>
        /// <param name="userName">FTP server user name.</param>
        /// <param name="password">FTP server user password.</param>
        /// <param name="folder">FTP folder where to store message.</param>
        internal GlobalMessageRuleAction_StoreToFtp(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,string host,int port,string userName,string password,string folder) : base(rule,owner,id,description)
        {
            m_Server   = host;
            m_Port     = port;
            m_UserName = userName;
            m_Password = password;
            m_Folder   = folder;

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
                        <Server></Server>
                        <Port></Server>
                        <User></User>
                        <Password></Password>
                        <Folder></Folder>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Server"  ,m_Server);
            table.Add("Port"    ,m_Port.ToString());
            table.Add("User"    ,m_UserName);
            table.Add("Password",m_Password);
            table.Add("Folder"  ,m_Folder);

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.StoreToFTPFolder; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets FTP server where to store message.
        /// </summary>
        public string Server
        {
            get{ return m_Server; }

            set{
                if(m_Server != value){
                    m_Server = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets FTP server port.
        /// </summary>
        public int Port
        {
            get{ return m_Port; }

            set{
                if(m_Port != value){
                    m_Port = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets FTP server user name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }

            set{
                if(m_UserName != value){
                    m_UserName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets FTP server user password.
        /// </summary>
        public string Password
        {
            get{ return m_Password; }

            set{
                if(m_Password != value){
                    m_Password = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets FTP folder where to store message.
        /// </summary>
        public string Folder
        {
            get{ return m_Folder; }

            set{
                if(m_Folder != value){
                    m_Folder = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
