using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Extended treeview. 
    /// </summary>
    public class WTreeView : TreeView
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WTreeView() : base()
        {
            this.AfterCheck += new TreeViewEventHandler(WTreeView_AfterCheck);
        }


        #region Events Handling
                
        private void WTreeView_AfterCheck(object sender,TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            foreach(TreeNode currentNode in node.Nodes){
                currentNode.Checked = node.Checked;
            }
        }

        #endregion

    }
}
