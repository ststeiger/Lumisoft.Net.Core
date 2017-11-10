using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Globalization;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Extended ListView control. Supports sorting.
    /// </summary>
    public class WListView : ListView
    {
        private ColumnHeader m_pSortingColumn = null;
        private SortOrder    m_SortOrder      = SortOrder.None;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WListView()
        {   
            //this.OwnerDraw = true;
            //this.DoubleBuffered = true;
        }


        #region method override OnColumnClick

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            ColumnHeader sortingColumn = this.Columns[e.Column];

            // Same column, switch sort order
            if(m_pSortingColumn != null && sortingColumn == m_pSortingColumn){
                if(m_SortOrder == SortOrder.Descending || m_SortOrder == SortOrder.None){
                    m_SortOrder = SortOrder.Ascending;
                }
                else{
                    m_SortOrder = SortOrder.Descending;
                }
            }
            else{                
                m_SortOrder = SortOrder.Ascending;
            }

            m_pSortingColumn = sortingColumn;
            SortItems();

            base.OnColumnClick(e);
        }

        #endregion

        #region method override OnDrawColumnHeader
/*
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            using(StringFormat sf = new StringFormat()){
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch(e.Header.TextAlign){
                    case HorizontalAlignment.Left:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                // Draw the standard header background.
                e.DrawBackground();

                // Draw the header text
                Rectangle textRect = new Rectangle(e.Bounds.X + 2,e.Bounds.Y + 2,e.Bounds.Width - 4,e.Bounds.Height - 4);
                e.Graphics.DrawString(e.Header.Text,e.Font,new SolidBrush(e.ForeColor),textRect,sf);                
            }
                        
            // Draw sorting arrow
            if(m_pSortingColumn == e.Header){ 
                Rectangle arrowRect = new Rectangle(e.Bounds.X + e.Bounds.Width - 12,e.Bounds.Y + 6,7,4);
                if(m_SortOrder == SortOrder.Ascending){                    
                    e.Graphics.FillPolygon(
                        new SolidBrush(Color.Gray),
                        new Point[]{
                            new Point(arrowRect.X,arrowRect.Y),
                            new Point(arrowRect.Right,arrowRect.Y),
                            new Point(arrowRect.X + (arrowRect.Width) / 2,arrowRect.Bottom)
                        }
                    );
                }
                else if(m_SortOrder == SortOrder.Descending){
                    e.Graphics.FillPolygon(
                        new SolidBrush(Color.Gray),
                        new Point[]{
                            new Point(arrowRect.X + (arrowRect.Width) / 2,arrowRect.Y - 1),
                            new Point(arrowRect.X - 1,arrowRect.Bottom),
                            new Point(arrowRect.Right,arrowRect.Bottom)             
                        }
                    );
                }
            }
        }
*/
        #endregion

        #region method override OnDrawItem
/*
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawItem(e);
        }
*/
        #endregion

        #region method OnDrawSubItem
/*
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawSubItem(e);
        }
*/
        #endregion


        #region method SortItems

        /// <summary>
        /// Sorts items based on active sorting column and sort order.
        /// </summary>
        public void SortItems()
        {
            if(m_pSortingColumn == null){
                return;
            }
            
            this.BeginUpdate();
                        
            ListViewItem[] buffer = new ListViewItem[this.Items.Count];
            this.Items.CopyTo(buffer,0);
            List<ListViewItem> items = new List<ListViewItem>(buffer);

            for(int z=0;z<items.Count;z++){
                bool changed = false;
                for(int i=0;i<items.Count - 1;i++){
                    ListViewItem currentItem = items[i];
                    ListViewItem nextItem    = items[i + 1];

                    if(m_SortOrder == SortOrder.Ascending){
                        if(currentItem.SubItems[m_pSortingColumn.Index].Text.CompareTo(nextItem.SubItems[m_pSortingColumn.Index].Text) > 0){                            
                            items.Remove(currentItem);
                            items.Insert(i + 1,currentItem);
                            changed = true;
                        }
                    }
                    else{
                        if(nextItem.SubItems[m_pSortingColumn.Index].Text.CompareTo(currentItem.SubItems[m_pSortingColumn.Index].Text) > 0){
                            items.Remove(currentItem);
                            items.Insert(i + 1,currentItem);
                            changed = true;
                        }
                    }                    
                }

                // All sorted, no need to check futher
                if(!changed){
                    break;
                }
            }

            this.Items.Clear();
            this.Items.AddRange(items.ToArray());
            this.EndUpdate();
            this.Refresh();
        }

        #endregion
    }
}
