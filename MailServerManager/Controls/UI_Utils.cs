using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// UI related utility methods.
    /// </summary>
    public class UI_Utils
    {
        #region static method GetGrayImage

        /// <summary>
        /// Creates gray scale image from the specified image.
        /// </summary>
        /// <param name="image">IMage to convert.</param>
        /// <returns>Returns gray scale image.</returns>
        public static Image GetGrayImage(Image image)
        {
            if(image == null){
                throw new ArgumentNullException("image");
            }

            Image grayImage = (Image)image.Clone();
            using(Graphics g = Graphics.FromImage(grayImage)){
                ControlPaint.DrawImageDisabled(g,image,0,0,Color.Transparent);
            }

            return grayImage;
        }

        #endregion
    }
}
