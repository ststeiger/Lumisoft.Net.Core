using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleAction_ExecuteProgram object represents user message rule "Execute Program" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_ExecuteProgram : UserMessageRuleActionBase
    {
        private string m_Program     = "";
        private string m_ProgramArgs = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal UserMessageRuleAction_ExecuteProgram(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(UserMessageRuleAction_enum.ExecuteProgram,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <Program></Program>
                        <Arguments></Arguments>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Program     = table.GetValue("Program");
            m_ProgramArgs = table.GetValue("Arguments");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="program">Program to execute.</param>
        /// <param name="programArgs">Executable program arguments.</param>
        internal UserMessageRuleAction_ExecuteProgram(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,string program,string programArgs) : base(UserMessageRuleAction_enum.ExecuteProgram,rule,owner,id,description)
        {
            m_Program     = program;
            m_ProgramArgs = programArgs;
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
                        <Program></Program>
                        <Arguments></Arguments>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Program"  ,m_Program);
            table.Add("Arguments",m_ProgramArgs);

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets program to execute.
        /// </summary>
        public string Program
        {
            get{ return m_Program; }

            set{
                if(m_Program != value){
                    m_Program = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets executable program arguments.
        /// </summary>
        public string ProgramArguments
        {
            get{ return m_ProgramArgs; }

            set{
                if(m_ProgramArgs != value){
                    m_ProgramArgs = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
