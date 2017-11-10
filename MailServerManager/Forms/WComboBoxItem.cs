using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Implements combox item with .Tag property support.
    /// </summary>
    public class WComboBoxItem
    {
        private string m_Text = "";
        private object m_Tag  = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">ComboBox item text.</param>
        public WComboBoxItem(string text)
        {
            m_Text = text;
        }

        /// <summary>
        /// ComboBox item with .Tag.
        /// </summary>
        /// <param name="text">ComboBox item text.</param>
        /// <param name="tag">User data.</param>
        public WComboBoxItem(string text,object tag)
        {
            m_Text = text;
            m_Tag  = tag;
        }


        #region override method ToString

        /// <summary>
        /// Returns combobox item text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_Text;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets combobox item text.
        /// </summary>
        public string Text
        {
            get{ return m_Text; }
        }

        /// <summary>
        /// Gets or sets combobox item tag (user data).
        /// </summary>
        public object Tag
        {
            get{ return m_Tag; }

            set{ m_Tag = value; }
        }

        #endregion

    }
}
