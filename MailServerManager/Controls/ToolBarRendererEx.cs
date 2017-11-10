using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Custom toobar renderer.
    /// </summary>
    internal class ToolBarRendererEx : ToolStripProfessionalRenderer
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ToolBarRendererEx()
        {
        }

        /// <summary>
        /// Just won't draw border.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {            
        }
        
        /// <summary>
        /// Just don't render background.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {            
        }   
    }

}
