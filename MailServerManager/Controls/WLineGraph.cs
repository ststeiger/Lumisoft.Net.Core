using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Simple line graph control.
    /// </summary>
    public class WLineGraph : Control
    {
        private List<Color> m_pLines       = null;
        private List<int[]> m_pPoints      = null;
        private int         m_CellOffset   = 0;
        private int         m_CellSize     = 12;
        private bool        m_AutoMaxValue = false;
        private int         m_MaxValue     = 100;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WLineGraph()
        {
            m_pLines  = new List<Color>();
            m_pPoints = new List<int[]>();

            // TODO: Mono won't support DoubleBuffered 
            try{
                this.GetType().GetProperty("DoubleBuffered",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetSetMethod(true).Invoke(this,new object[]{true});
            }
            catch{
            }            
        }


        #region protected override OnPaint

        /// <summary>
        /// Draw graph.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            base.OnPaint(e);

            // Calculate UI info
            int     numberOfPoints = this.Width / 3;
            decimal yScale         = this.Height / (decimal)m_MaxValue;
            // Remove queued out points what aren't visible any more
            while(m_pPoints.Count > numberOfPoints){
                m_pPoints.RemoveAt(0);
            }
                                                
            // Fill background
            g.Clear(Color.Black);

            // Draw grid horizontal lines
            for(int i = 0;i < this.Height;i += m_CellSize){
                g.DrawLine(new Pen(new SolidBrush(Color.Green)),0,i,this.Width,i);
            }
            // Draw grid vertical lines
            for(int i = this.Width;i > 0;i -= m_CellSize){
                g.DrawLine(new Pen(new SolidBrush(Color.Green)),i - m_CellOffset,0,i - m_CellOffset,this.Height);
            }

            // Draw lines
            for(int l=0;l<m_pLines.Count;l++){
                Color lineColor = m_pLines[l];

                // Draw line
                int lastPointY  = this.Height;
                int pointStartX = this.Width;
                for(int i = m_pPoints.Count - 1;i >- 1;i--){
                    g.DrawLine(new Pen(new SolidBrush(lineColor)),pointStartX - 3,this.Height - (int)(m_pPoints[i][l] * yScale),pointStartX,lastPointY);
                    pointStartX -= 3;
                    lastPointY   = this.Height - (int)(m_pPoints[i][l] * yScale);
                }
            }
        }

        #endregion


        #region method AddLine

        /// <summary>
        /// Adds new line to graph.
        /// </summary>
        public void AddLine(Color lineColor)
        {
            m_pLines.Add(lineColor);
        }

        #endregion

        #region method AddValue

        /// <summary>
        /// Adds next step value to graph.
        /// </summary>
        /// <param name="values">If AutoMaxValue disabled, values must be between 0 > MaximumValue, other wise 0 > ... .</param>
        public void AddValue(int[] values)
        {
            if(m_pLines.Count != values.Length){
                throw new ArgumentException("You must provide values for all lines, Lines count must equal values.Lengh !");
            }
            foreach(int value in values){
                if(value < 0){
                    throw new ArgumentException("Value must be between > 0 !");
                }
                if(!m_AutoMaxValue && value > m_MaxValue){
                    throw new ArgumentException("Value must be between <= " + m_MaxValue + " (MaximumValue) !");
                }
            }

            m_pPoints.Add(values);

            // Get auto maximum value
            if(m_AutoMaxValue){
                m_MaxValue = 1;
                foreach(int[] points in m_pPoints){
                    foreach(int value in points){
                        if(value > m_MaxValue){
                            m_MaxValue = value;
                        }
                    }
                }
            }           
            
            // Move cells offset
            if(m_CellOffset < m_CellSize){
                m_CellOffset += 3;
            }
            else{
                m_CellOffset = 3;
            }

            // Force to redraw graph
            this.Refresh();
        }

        #endregion

        #region method ClearValues

        /// <summary>
        /// Clears all lines from graph.
        /// </summary>
        public void ClearValues()
        {
            m_pPoints.Clear();
            this.Refresh();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets if maximum value is dynamically controlled.
        /// </summary>
        public bool AutoMaxValue
        {
            get{ return m_AutoMaxValue; }

            set{ m_AutoMaxValue = value; }
        }

        /// <summary>
        /// Gets or sets maximum value that can be in graph. This value must be >= 1.
        /// If AutoMaxValue is enabled, this property is dynamically controlled.
        /// </summary>
        public int MaximumValue
        {
            get{ return m_MaxValue; }

            set{
                if(value < 1){
                    throw new ArgumentException("MaximumValue value must be >= 1 !");
                }

                m_MaxValue = value;
            }
        }

        #endregion

    }
}
