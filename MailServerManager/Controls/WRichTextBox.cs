using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Extend RichTextBox. Adds support for this.SuspendPaint.
    /// </summary>
    public class WRichTextBox : RichTextBox
    {
        private bool m_SuspendPaint = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WRichTextBox()
        {
        }


        #region method override WndProc

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // WM_PAINT = 0x00f
            if(m.Msg == 0x00f){
                if(m_SuspendPaint){
                    m.Result = IntPtr.Zero;
                }
                else{
                    base.WndProc(ref m);
                }
            }
            else{
                base.WndProc(ref m);
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets if painting is suspended.
        /// </summary>
        public bool SuspendPaint
        {
            get{ return m_SuspendPaint; }

            set{ 
                m_SuspendPaint = value; 

                if(!value){
                    this.Refresh();
                }
            }
        }

        #endregion

    }
}
